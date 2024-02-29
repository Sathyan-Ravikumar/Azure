using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    internal class TopicSender
    {
        private ServiceBusSender m_Sender;

        public TopicSender(string connectionString,string topicName)
        {
            var client = new ServiceBusClient(connectionString);
            m_Sender = client.CreateSender(topicName);
        }

        public async Task SendOrderMessage(Order order)
        {
            Console.WriteLine($"{order.ToString()}");

            //serialize the order to json
            var orderJson = JsonConvert.SerializeObject(order);

            //create a message containing the serialized order json
            var message = new ServiceBusMessage(orderJson);

            message.ApplicationProperties.Add("region", order.Region);
            message.ApplicationProperties.Add("items", order.Items);
            message.ApplicationProperties.Add("value", order.Value);
            message.ApplicationProperties.Add("loyalty", order.HasLoyaltyCard);

            //Set the correlationId
            message.CorrelationId = order.Region;

            //send the message
            await m_Sender.SendMessageAsync(message); 


        }

       
    }
}
