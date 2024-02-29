using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    internal class SubscriptionReceiver
    {
        private ServiceBusReceiver serviceBusReceiver;

        public SubscriptionReceiver(string connectionstring, string Topicname, string subsname)
        {
            var client = new ServiceBusClient(connectionstring);
            serviceBusReceiver = client.CreateReceiver(Topicname, subsname);
        }

        public async Task ReceiveAndProcessOrder()
        {
            var message = await serviceBusReceiver.ReceiveMessageAsync();
            foreach(var property in message.ApplicationProperties)
            {
                Console.WriteLine($"{property.Key}        -      {property.Value}");
            }
            Console.WriteLine(message.Body.ToString());
            Console.WriteLine();

            await serviceBusReceiver.CompleteMessageAsync(message);
        }
    }
}
