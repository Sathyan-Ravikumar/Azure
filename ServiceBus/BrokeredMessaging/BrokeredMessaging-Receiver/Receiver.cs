using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace BrokeredMessaging_Receiver
{
    internal class Receiver
    {
        static string ConnectionString = "Endpoint=sb://simplemessagingusingservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SfcZIuOGN1c1dtqrJHlLg5hQGyQCWs39s+ASbFnifXE=";
        static string QueueName = "firstQueue";
        static async Task Main(string[] args)
        {
            var client = new ServiceBusClient(ConnectionString);

            var receiver = client.CreateReceiver(QueueName);

            Console.WriteLine("Receiving Messages ...");
            var message = await receiver.ReceiveMessageAsync();
            var receive = JsonSerializer.Deserialize<Data>(message.Body);
            await receiver.CompleteMessageAsync(message);
            Console.Write(message.Body.ToString());
            Console.WriteLine();
            Console.WriteLine("Received Your Message");

           /* while (true)
            {
                var message = await receiver.ReceiveMessageAsync();
                
                if(message != null)
                {
                    Console.Write(message.Body.ToString());

                    await receiver.CompleteMessageAsync(message);

                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Received Your Message");
                    break;
                }
               
            }*/
            await receiver.CloseAsync();

        }
    }
}
