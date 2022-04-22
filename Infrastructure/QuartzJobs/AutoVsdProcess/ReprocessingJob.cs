using System;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.QuartzJobs.AutoVsdProcess
{
    public class ReprocessingJob: IJob
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IAutoVsdProcessService _autoVsdProcessService;
        private readonly IAutoVsdProcessDataService _autoVsdProcessDataService;
        private readonly ILogger<ReprocessingJob> _logger;

        public ReprocessingJob(ISchedulerFactory schedulerFactory,
            IAutoVsdProcessService autoVsdProcessService,
            IAutoVsdProcessDataService autoVsdProcessDataService, ILogger<ReprocessingJob> logger)
        {
            _schedulerFactory = schedulerFactory;
            _autoVsdProcessService = autoVsdProcessService;
            _autoVsdProcessDataService = autoVsdProcessDataService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _autoVsdProcessService.ProcessVsd(context.CancellationToken);

            if (DateTime.Now < _autoVsdProcessDataService.AutoProcessEnd
                && _autoVsdProcessDataService.Users.Count > 0)
            {
                var scheduler = await _schedulerFactory.GetScheduler();

                var job = JobBuilder.Create<ReprocessingJob>().Build();

                var trigger = TriggerBuilder.Create().StartNow().Build();
                
                _logger.LogInformation("Перезапуск процедуры автогашения.");

                await scheduler.ScheduleJob(job, trigger);
            }
            else
                _logger.LogInformation("Завершение перезапуска процедуры автогашения.");
        }
    }
}