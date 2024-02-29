using System.Collections.Generic;
using Azure.Messaging.ServiceBus;
using System.Threading.Tasks;


namespace TopicsAndSubscriptions
{
    internal class TopicsAndSubscriptionConsole
    {

        private static string ServiceBusConnection = "Endpoint=sb://topicsandsubscriptions.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=nBV8J8WemvPz+HNZRCwAA7yIPVgHLCOfI+ASbOTe0PY=";
        private static string OrdersTopicName = "Orders";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Topic and Subscription ");

            Console.WriteLine("Press enter to create topic and subscription...");
            Console.ReadLine();
            await CreateTopicsAndSubscriptions();

            Console.WriteLine("Press enter to send order messages...");
            Console.ReadLine();
            await SendOrderMessages();

            Console.WriteLine("Press enter to receive order messages...");
            Console.ReadLine();
            await ReceiveOrderFromAllSubcriptions();


             Console.WriteLine("Topics and Subscriptions Console Complete");
            Console.ReadLine();
            await SendOrderMessages();


        }

        static async Task CreateTopicsAndSubscriptions()
        {
            var manager = new Manager(ServiceBusConnection);

            await manager.CreateTopic(OrdersTopicName);
            await manager.CreateSubscription(OrdersTopicName,"AllOrders");

            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicName, "UsaOrders", "region = 'USA'");
            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicName, "EuOrders", "region = 'EU'");

            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicName, "LargeOrders", "items > 30");
            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicName, "HighValueOrders", "value > 500");

            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicName, "LoyaltyCardOrders", "loyalty = true AND region = 'USA'");
            await manager.CreateSubscriptionWithCorrelationFilter(OrdersTopicName, "UkOrders", "UK");

        }

        static async Task SendOrderMessages()
        {
            var orders = CreateTestOrder();
            var sender = new TopicSender(ServiceBusConnection, OrdersTopicName);
            foreach(var order in orders)
            {
                await sender.SendOrderMessage(order);
            }
          //  await sender.Close();
        }

        static async Task ReceiveOrderFromAllSubcriptions()
        {
            /*var manager = new Manager(ServiceBusConnection);

           var subscriptions = await manager.GetSubscriptionsForTopic(OrdersTopicName);

             foreach( var subscription in subscriptions)
             {
                 Console.WriteLine($"Receive order from {subscription}...");

                 var receiver = new SubscriptionReceiver(ServiceBusConnection, OrdersTopicName, subscription);
                 await receiver.ReceiveAndProcessOrder();
                 //await receiver.Close();
             }*/

            var subscription = "EUOrders";
            Console.WriteLine($"Receive order from {subscription}...");

            var receiver = new SubscriptionReceiver(ServiceBusConnection, OrdersTopicName, subscription);
            await receiver.ReceiveAndProcessOrder();
        }

        static List<Order> CreateTestOrder()
        {
            var order = new List<Order>();

            order.Add(new Order()
            {
                Name = "Loyal Customer",
                Value = 19.99,
                Region = "USA",
                Items = 1,
                HasLoyaltyCard = true
            });
            order.Add(new Order()
            {
                Name = "Large Order",
                Value = 49.99,
                Region = "EU",
                Items = 50,
                HasLoyaltyCard = false
            });
            order.Add(new Order()
            {
                Name = "High Value",
                Value = 749.45,
                Region = "USA",
                Items = 45,
                HasLoyaltyCard = false
            });
            order.Add(new Order()
            {
                Name = "UK Order",
                Value = 49.49,
                Region = "UK",
                Items = 3,
                HasLoyaltyCard = false
            });
            order.Add(new Order()
            {
                Name = "Loyal Europe",
                Value = 49.45,
                Region = "EU",
                Items = 3,
                HasLoyaltyCard = true
            });

            return order;
        }
    }
}
