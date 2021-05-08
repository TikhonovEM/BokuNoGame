using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace BokuNoGame.IntegrationServices
{
    public static class SteamIntegratonScheduler
    {
        public static async void Start(IServiceProvider serviceProvider)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.JobFactory = serviceProvider.GetService<IntegrationJobFactory>();
            await scheduler.Start();

            var jobDetail = JobBuilder.Create<SteamIntegrationJob>().Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithCronSchedule(Startup.Configuration[$"IntegrationSettings:SteamIntegrationSettings:CronSchedule"])
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger);

        }
    }
}
