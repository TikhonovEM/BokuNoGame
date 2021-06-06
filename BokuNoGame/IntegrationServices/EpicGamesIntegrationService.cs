using BokuNoGame.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.IntegrationServices
{
    public class EpicGamesIntegrationService : IBaseIntegrationService
    {
        private readonly ILogger<SteamIntegratonService> _logger;
        private readonly AppDBContext _appDBContext;
        public EpicGamesIntegrationService(ILogger<SteamIntegratonService> logger, AppDBContext appDBContext)
        {
            _logger = logger;
            _appDBContext = appDBContext;
        }
        public string ExternalSystemDescriptor { get; set; } = "Epic Games";

        public async Task GetActualDataAsync()
        {
            var scriptFolder = Startup.Configuration["IntegrationSettings:EpicGamesIntegrationSettings:ScriptFolderPath"];
            var script = Startup.Configuration["IntegrationSettings:EpicGamesIntegrationSettings:ScriptName"];

            var pythonPath = Startup.Configuration["IntegrationSettings:EpicGamesIntegrationSettings:PythonPath"];

            var slugs = _appDBContext.IntegrationInfos
                .Where(ii => ii.ExternalSystemDescriptor.Equals(ExternalSystemDescriptor))
                .Select(ii => ii.ExternalGameIdStr);

            var p = new Process
            {
                StartInfo = new ProcessStartInfo(pythonPath, Path.Combine(scriptFolder, script))
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();

            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            Console.WriteLine(output);
        }

        public async Task SaveChangesAsync()
        {
            await _appDBContext.SaveChangesAsync();
        }
    }
}
