using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Steam.Models.SteamStore;
using SteamWebAPI2.Utilities;
using SteamWebAPI2.Interfaces;
using BokuNoGame.Models;
using System.Net;

namespace BokuNoGame.IntegrationServices
{
    public class SteamIntegratonService : IBaseIntegrationService
    {
        private readonly ILogger<SteamIntegratonService> _logger;
        private readonly AppDBContext _appDBContext;
        public string ExternalSystemDescriptor { get; } = "Steam";        
        private readonly string _steamApiKey = Startup.Configuration["IntegrationSettings:SteamIntegrationSettings:SteamWebApiKey"];
        private List<StoreAppDetailsDataModel> AppDetails = new List<StoreAppDetailsDataModel>();

        public SteamIntegratonService(ILogger<SteamIntegratonService> logger, AppDBContext appDBContext)
        {
            _logger = logger;
            _appDBContext = appDBContext;
        }

        public async Task GetActualDataAsync()
        {
            _logger.LogInformation("Start integration session with Steam API");
            var steamWebInterfaceFactory = new SteamWebInterfaceFactory(_steamApiKey);
            var steamApps = steamWebInterfaceFactory.CreateSteamWebInterface<SteamApps>();

            var ids = _appDBContext.IntegrationInfos
                .Where(ii => ii.ExternalSystemDescriptor.Equals(ExternalSystemDescriptor))
                .Select(ii => ii.ExternalGameId);

            var listResponse = await steamApps.GetAppListAsync();
            var appInfoList = listResponse.Data.Where(a => !ids.Any(id => id == a.AppId));

            var appsCountStr = Startup.Configuration["IntegrationSettings:SteamIntegrationSettings:MaxPacketSize"];
            var appsCount = 0;
            var isMaxSizeExists = false;
            if (appsCountStr != null)
            {
                appsCount = Convert.ToInt32(appsCountStr);
                isMaxSizeExists = true;
            }
            var steamStoreInterface = steamWebInterfaceFactory.CreateSteamStoreInterface();
            var lang = "russian";
            foreach (var app in appInfoList)
            {
                StoreAppDetailsDataModel appDetails = null;
                try
                {
                    appDetails = await steamStoreInterface.GetStoreAppDetailsAsync(app.AppId, lang);
                }
                catch(NullReferenceException)
                {
                    _logger.LogError("Не удалось получить информацию о приложении {0}({1})", app.Name, app.AppId);
                    continue;
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Произошла непредвиденная ошибка: ");
                    continue;
                }
                // Skip DLC
                if (appDetails.Type.Equals("game"))
                {
                    AppDetails.Add(appDetails);
                    if (isMaxSizeExists && AppDetails.Count >= appsCount)
                        break;
                }
            }
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Start migration to DB");
            foreach (var appDetail in AppDetails)
            {
                try
                {
                    var game = new Game();
                    game.Name = appDetail.Name;
                    game.Description = appDetail.DetailedDescription;
                    game.Publisher = appDetail.Publishers.FirstOrDefault();
                    game.Developer = appDetail.Developers.FirstOrDefault();
                    var genres = appDetail.Genres;
                    var genreValue = Genre.Default;
                    foreach (var genre in genres)
                    {
                        // Поиск первого подходящего жанра.
                        genreValue = genre.Description switch
                        {
                            "Экшены" => Genre.Action,
                            "Симуляторы" => Genre.Simulation,
                            "Стратегии" => Genre.Strategy,
                            "Ролевые игры" => Genre.RPG,
                            "Головоломки" => Genre.Puzzle,
                            "Казуальные игры" => Genre.Arcade,
                            "Гонки" => Genre.Race,
                            _ => Genre.Default
                        };
                        if (genreValue != Genre.Default)
                            break;
                    }
                    game.Genre = genreValue;
                    game.ReleaseDate = Convert.ToDateTime(appDetail.ReleaseDate.Date);
                    game.AgeRating = appDetail.RequiredAge.ToString();
                    using (var webclient = new WebClient())
                    {
                        game.Logo = webclient.DownloadData(appDetail.HeaderImage);
                    }
                    await _appDBContext.Games.AddAsync(game);
                    await _appDBContext.SaveChangesAsync();
                    _logger.LogInformation($"Created game(Id = {game.Name})");

                    var integrationInfo = new IntegrationInfo();
                    integrationInfo.ExternalSystemDescriptor = ExternalSystemDescriptor;
                    integrationInfo.ExternalGameId = Convert.ToInt32(appDetail.SteamAppId);
                    integrationInfo.InternalGameId = game.Id;
                    integrationInfo.Date = DateTime.Now;
                    await _appDBContext.IntegrationInfos.AddAsync(integrationInfo);
                    await _appDBContext.SaveChangesAsync();
                    _logger.LogInformation($"Created integration info(Id = {integrationInfo.InternalGameId})");
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Произошла ошибка при миграции в БД");
                }
            }
        }
    }
}
