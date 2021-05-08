using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BokuNoGame.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Web;

namespace BokuNoGame
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                var webHost = BuildWebHost(args);

                using (var scope = webHost.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        var userManager = services.GetRequiredService<UserManager<User>>();
                        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                        var context = services.GetService<AppDBContext>();
                        await Services.IdentityInitializer.InitializeAsync(userManager, roleManager, context);
                        IntegrationServices.SteamIntegratonScheduler.Start(services);
                    }
                    catch
                    {

                    }

                }

                webHost.Run();
            }
            catch (Exception e)
            {
                //NLog: catch setup errors
                logger.Error(e, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .UseNLog()
            .Build();
    }
}
