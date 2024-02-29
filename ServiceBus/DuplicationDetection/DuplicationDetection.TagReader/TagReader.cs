using Azure.Messaging.ServiceBus;
using DuplicationDetection.Config;
using DuplicationDetection.Data;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace DuplicationDetection.TagReader
{
    internal class TagReader
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Tag Reader Console");
            var serviceBusClient = new ServiceBusClient(AccountDetails.ConnectionString);
            var messageSender = serviceBusClient.CreateSender(AccountDetails.QueueName);

            RfidTag[] orderItem = new RfidTag[]
            {
                new RfidTag() { Product = "Ball",Price = 4.99 },
                new RfidTag() { Product = "Whistle",Price = 6.08 },
                new RfidTag() { Product = "Bat",Price = 8.99 },
                new RfidTag() { Product = "Gloves",Price = 41.99 },
                new RfidTag() { Product = "Gloves",Price = 41.99 },
                new RfidTag() { Product = "Bat",Price = 8.99 },
                new RfidTag() { Product = "Cap",Price = 14.99 },
                new RfidTag() { Product = "Cap",Price = 14.99 }

            };
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Order Contains {0} items.", orderItem.Length);
            Console.ForegroundColor = ConsoleColor.Yellow;


            double orderTotal = 0.0;

            foreach(RfidTag tag in orderItem)
            {
                Console.WriteLine("{0} - ${1}",tag.Product,tag.Price);

                orderTotal += tag.Price;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Order Value =${0}.", orderTotal);

            Console.WriteLine();
            Console.ResetColor();

            Console.WriteLine("Press Enter to Scan.....");
            Console.ReadLine();

            Random random = new Random(DateTime.Now.Millisecond);

            int sentCount = 0;
            int position = 0;

            Console.WriteLine("Reading tags...");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;

            var sessionId = Guid.NewGuid().ToString();
            Console.WriteLine($"SessionId: {sessionId}");

            while (position<8)
            {
                RfidTag rfidTag= orderItem[position];

                var orderJson = JsonConvert.SerializeObject(rfidTag);
                var tagReadMessage = new ServiceBusMessage(orderJson);

                tagReadMessage.MessageId = rfidTag.TagId;
                
                tagReadMessage.SessionId = sessionId;

                await messageSender.SendMessageAsync(tagReadMessage);
               // Console.WriteLine($"Sent: {orderItem[position].Product}");

                Console.WriteLine($"Sent: {orderItem[position].Product} - MessageId: {tagReadMessage.MessageId}");

                // Random duplicate message
                if(random.NextDouble() > 0.4 ) position++;
                sentCount++;

                Thread.Sleep(100);
            }

            Console.ForegroundColor= ConsoleColor.Green;
            Console.WriteLine("{0} total tag reads", sentCount); Console.WriteLine();
            Console.ResetColor();
            Console.ReadLine();
        }
    }
}
