using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using DuplicationDetection.Config;
using DuplicationDetection.Data;
using Newtonsoft.Json;

namespace DuplicationDetection.Checkout
{
    internal class Program
    {
        private static double totalPrice = 0;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Checkout Console..");

            var serviceBusAdministrationClient = new ServiceBusAdministrationClient(AccountDetails.ConnectionString);

            if (await serviceBusAdministrationClient.QueueExistsAsync(AccountDetails.QueueName))
            {
                await serviceBusAdministrationClient.DeleteQueueAsync(AccountDetails.QueueName);
            }

            var rfidCreateQueueOption = new CreateQueueOptions(AccountDetails.QueueName)
            {
                RequiresDuplicateDetection = true,
                DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10),
                RequiresSession = true
            };

            await serviceBusAdministrationClient.CreateQueueAsync(rfidCreateQueueOption);


            /*var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1,
                MaxAutoLockRenewalDuration = TimeSpan.FromSeconds(10)
            };*/


            var serviceBusClient = new ServiceBusClient(AccountDetails.ConnectionString);



            // var messageReceiver = serviceBusClient.CreateReceiver(AccountDetails.QueueName);

            Console.WriteLine("Receiving tag read messages ...");

            while(true)
            {
                int receivedcount = 0;
                double bill = 0.0;
                Console.ForegroundColor = ConsoleColor.Cyan;

                // message session
                var sessionReceiver = await serviceBusClient.AcceptNextSessionAsync(AccountDetails.QueueName);
                Console.WriteLine("Accepted session : " + sessionReceiver.SessionId);

                
                Console.ForegroundColor = ConsoleColor.Yellow;

                while(true)
                {
                    // var receivedTagsMessage  = await messageReceiver.ReceiveMessagesAsync(TimeSpan.FromSeconds(5));

                    var receivedTags = await sessionReceiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1));

                    if (receivedTags != null)
                    {
                        var rfidJson = receivedTags.Body.ToString();
                        var rfidTag = JsonConvert.DeserializeObject<RfidTag>(rfidJson);
                        Console.WriteLine("Bill for {0} ", rfidTag.Product);
                        receivedcount++;
                        bill += rfidTag.Price;

                        //await messageReceiver.CompleteMessageAsync(receivedTags);
                        await sessionReceiver.CompleteMessageAsync(receivedTags);

                    }
                    else
                    {
                        await sessionReceiver.CloseAsync();
                        break;
                    }
                }

                if(receivedcount > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Customer Bill ${0} for {1} items.",bill,receivedcount);
                    Console.WriteLine();
                    Console.ResetColor();
                }

            }


            


           /* var processor = receivingConnection.CreateProcessor(AccountDetails.QueueName, options);

            processor.ProcessMessageAsync += MessageAsync;

            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();

            Console.WriteLine("Receiving Tags..");
            Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Total Price: {totalPrice}");

            await processor.StopProcessingAsync();
            await processor.CloseAsync();

            static async Task MessageAsync(ProcessMessageEventArgs args)
            {

                var message = JsonConvert.DeserializeObject<RfidTag>(args.Message.Body.ToString());

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Recived product : {message.Product} for price : {message.Price}");

                totalPrice += message.Price;
                await args.CompleteMessageAsync(args.Message);
            }


            static async Task ErrorHandler(ProcessErrorEventArgs args)
            {
                Console.WriteLine(args.Exception.Message);
            }*/

        }
    }
}
