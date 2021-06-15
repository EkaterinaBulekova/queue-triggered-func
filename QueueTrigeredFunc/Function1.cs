using System;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace QueueTrigeredFunc
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([QueueTrigger("incoming-documents-notifications", Connection = "storage-connection-string")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            DocumentInfo documentInfo = JsonSerializer.Deserialize<DocumentInfo>(myQueueItem);

            log.LogInformation(documentInfo.DocumentSummary);
        }
    }
}
