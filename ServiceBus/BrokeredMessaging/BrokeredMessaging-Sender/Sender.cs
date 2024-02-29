using Azure.Core;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace BrokeredMessaging.Sender
{
    public class Sender
    {
        static string ConnectionString = "Endpoint=sb://simplemessagingusingservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SfcZIuOGN1c1dtqrJHlLg5hQGyQCWs39s+ASbFnifXE=";
        static string QueueName = "firstQueue";
        static string message;
        static string Name;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter Your Name: ");
            Name = Console.ReadLine();
            Console.WriteLine("Enter Your Message: ");
            message = Console.ReadLine();

            var data = new Data();
            data.Name = Name;
            data.Date = DateTime.Now;
            data.Message = message;



            var jsonData = JsonSerializer.Serialize(data);
            var client = new ServiceBusClient(ConnectionString);
            var sender = client.CreateSender(QueueName);
            var send = new ServiceBusMessage(jsonData);
            Console.WriteLine("Sending Message...");
            await sender.SendMessageAsync(send);

            /*foreach (var character in message)
            {
                var send = new ServiceBusMessage(character.ToString());
                await sender.SendMessageAsync(send);
                Console.WriteLine($" Sent: {character}");
            }*/

            await sender.CloseAsync();
            Console.WriteLine("Sent Message...");
            Console.ReadLine();
        }
    }
}
