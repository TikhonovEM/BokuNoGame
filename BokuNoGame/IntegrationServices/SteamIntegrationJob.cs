using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace BokuNoGame.IntegrationServices
{
    public class SteamIntegrationJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public SteamIntegrationJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<IBaseIntegrationService>();
                await service.GetActualDataAsync();
                await service.SaveChangesAsync();
            }
        }
    }
}
