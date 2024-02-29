using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
namespace BrokeredMessaging_Message
{
    internal class Chat
    {
        static string ConnectionString = "Endpoint=sb://simplemessagingusingservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SfcZIuOGN1c1dtqrJHlLg5hQGyQCWs39s+ASbFnifXE=";
        static string TopicName = "firstTopic";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Notes: If you want to Leave the conversation Enter \"exit\" ");

            Console.WriteLine();
            Console.Write("Enter Your Name : ");
            var userName = Console.ReadLine();

            var serviceBusAdministrationClient = new ServiceBusAdministrationClient(ConnectionString);


            //create topic
            if (!await serviceBusAdministrationClient.TopicExistsAsync(TopicName))
            {
                await serviceBusAdministrationClient.CreateTopicAsync(TopicName);

            }

            //create temp subcription for the user 
            if (!await serviceBusAdministrationClient.SubscriptionExistsAsync(TopicName, userName))
            {
                var option = new CreateSubscriptionOptions(TopicName, userName)
                {
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(6)
                };
                await serviceBusAdministrationClient.CreateSubscriptionAsync(option);
            }

            // creating service bus client
            var serviceBusClient = new ServiceBusClient(ConnectionString);

            // creating service bus sender
            var serviceBusSender = serviceBusClient.CreateSender(TopicName);

            // create a message processor
            var processor = serviceBusClient.CreateProcessor(TopicName, userName);

            // add handler to process messages
            processor.ProcessMessageAsync += MessageHandler;

            //add handler to process any error
            processor.ProcessErrorAsync += ErrorHandler;

            // start message processor
            await processor.StartProcessingAsync();

            var helloMessage = new ServiceBusMessage($" {userName} has entered the conversation");
            await serviceBusSender.SendMessageAsync(helloMessage);

            while (true)
            {
                var text = Console.ReadLine();
                if (text == "exit")
                {
                    break;
                }

                var message = new ServiceBusMessage($" {userName}>  {text} ");
                await serviceBusSender.SendMessageAsync(message);
            }


            //user left message
            var left = new ServiceBusMessage($"{userName} has left the conversation");
            await serviceBusSender.SendMessageAsync(left);

            //stop the message processor
            await processor.StopProcessingAsync();

            //close processor and sender
            await processor.CloseAsync();
            await serviceBusSender.CloseAsync();


        }

        static async Task MessageHandler(ProcessMessageEventArgs margs)
        {
            //retrive and print the message
            var test = margs.Message.Body.ToString();
            Console.WriteLine(test);

            //complete the message
            await margs.CompleteMessageAsync(margs.Message);
        }

        static async Task ErrorHandler(ProcessErrorEventArgs eargs)
        {
           throw new NotImplementedException();
        }
    }
}
