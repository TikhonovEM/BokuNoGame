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
                catch(Exception e)
                {
                    _logger.LogError(e, "Не удалось получить информацию о приложении");
                    continue;
                }
                // Skip DLC
                if (!appDetails.Categories.Any(c => c.Id == 21))
                {
                    AppDetails.Add(appDetails);
                    if (isMaxSizeExists && AppDetails.Count >= appsCount)
                        break;
                }
            }

            _logger.LogInformation($"Found next apps: {string.Join("$$", AppDetails.Select(a => a.Name))}");
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("закончили интеграцию");
        }
    }
}
