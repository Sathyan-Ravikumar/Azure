using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public class Blobtrigger
    {
        private readonly ILogger<Blobtrigger> _logger;

        public Blobtrigger(ILogger<Blobtrigger> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Blobtrigger))]
        [ServiceBusOutput("blobtriggeroutputbind", Connection = "ServiceBusConnection")] 
        public async Task<string> Run([BlobTrigger("bolbcontainer/{name}", Connection = "BlobConnection")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
            return content + " - " + DateTime.Now;
        }
    }
}
