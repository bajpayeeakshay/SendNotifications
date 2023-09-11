using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SendNotification.Services.Services;
using Serilog;

namespace SendNotificationsTimerTrigger
{
    public class SendNotificationsFunction
    {
        private readonly INotificationProcessingService _notificationProcessingService;
        private readonly ILogger _logger;

        public SendNotificationsFunction(INotificationProcessingService notificationProcessingService, ILogger logger) {
            _notificationProcessingService = notificationProcessingService;
            _logger = logger;
        }

        [FunctionName(nameof(SendNotificationsFunction))]
        public async Task RunAsync([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer)
        {
            _logger.Information($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                // Process data using the injected service
                await _notificationProcessingService.ProcessDataAsync(myTimer.ScheduleStatus.Last);


                _logger.Information("Function completed successfully.");
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Error occurred");
            }

        }
    }
}
