using System;
using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public class TimerTrigger
    {
        private readonly ILogger _logger;

        public TimerTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TimerTrigger>();
        }

        [Function("TimerTriggerFunction")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
                        [BlobInput("bolbcontainer/Output.txt", Connection = "BlobConnection")] Stream blobStream)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            using var reader = new StreamReader(blobStream);
            var content = reader.ReadToEnd();
            _logger.LogInformation($"Content from blob: {content}");
        }
    }
}
