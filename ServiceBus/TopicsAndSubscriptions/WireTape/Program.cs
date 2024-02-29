using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace WireTape
{
    internal class Program
    {
        private static string ServiceBusConnection = "Endpoint=sb://topicsandsubscriptions.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=nBV8J8WemvPz+HNZRCwAA7yIPVgHLCOfI+ASbOTe0PY=";
        private static string OrdersTopicName = "Orders";
        static async Task Main(string[] args)
        {
            Console.WriteLine("WireTap ...");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press Enter to Activate Wire Tap");
            Console.ReadLine();

            var subscriptionName = $"wiretap-{Guid.NewGuid()}";

            var administrationClient = new ServiceBusAdministrationClient(ServiceBusConnection);
            await administrationClient.CreateSubscriptionAsync(new CreateSubscriptionOptions(OrdersTopicName, subscriptionName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(10)
            });

            var serviceBusClient = new ServiceBusClient(ServiceBusConnection);

            var receiver = serviceBusClient.CreateReceiver
                (OrdersTopicName, subscriptionName);

            Console.WriteLine($"Receiving on {subscriptionName}");
            Console.WriteLine();

            while (true)
            {
                var message = await receiver.ReceiveMessageAsync();

                if (message != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Received message ...");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Properties");
                    Console.ForegroundColor= ConsoleColor.Green;
                    Console.WriteLine($" ContentType           -     {message.ContentType }");
                    Console.WriteLine($" CorrelationId         -     {message.CorrelationId}");
                    Console.WriteLine($" ExpiresAt             -     {message.ExpiresAt}");
                    Console.WriteLine($" Label                 -     {message.Subject}");
                    Console.WriteLine($" MessageId             -     {message.MessageId}") ;
                    Console.WriteLine($" PartitionKey          -     {message.PartitionKey}");
                    Console.WriteLine($" ReplyTo               -     {message.ReplyTo}");
                    Console.WriteLine($" ReplyToSessionId      -     {message.ReplyToSessionId}");
                    Console.WriteLine($" ScheduledEnqueueTime  -     {message.ScheduledEnqueueTime}");
                    Console.WriteLine($" SessionId             -     {message.SessionId}");
                    Console.WriteLine($" TimeToLive            -     {message.TimeToLive}");
                    Console.WriteLine($" To                    -     {message.To}");
                    Console.WriteLine($" EnqueuedTime          -     {message.EnqueuedTime}");
                    Console.WriteLine($" SequenceNumber        -     {message.SequenceNumber}");
                    Console.WriteLine($" LockedUntil           -     {message.LockedUntil}");


                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("ApplicationProperties");
                    Console.ForegroundColor = ConsoleColor.White;

                    foreach(var property in message.ApplicationProperties)
                    {
                        Console.WriteLine($"  {property.Key}  -  {property.Value}");
                        
                    }

                    Console.ForegroundColor = ConsoleColor.Cyan;

                    Console.WriteLine("Body");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(message.Body.ToString());
                    Console.WriteLine();

                    await receiver.CompleteMessageAsync(message);
                }
            }
        }
    }
}
