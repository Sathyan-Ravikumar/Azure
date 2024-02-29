using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    internal class Manager
    {
        private ServiceBusAdministrationClient m_administrationClient;

        public Manager(string connectionString)
        {
            m_administrationClient  = new ServiceBusAdministrationClient(connectionString);
        }

        public async Task<TopicProperties> CreateTopic(string? topicName)
        {
            Console.WriteLine($"Creating Topic {topicName}");

            if(await m_administrationClient.TopicExistsAsync(topicName))
            {
                await m_administrationClient.DeleteTopicAsync(topicName);
            }

            return await m_administrationClient.CreateTopicAsync(topicName);
        }

        public async Task<SubscriptionProperties> CreateSubscription(string topicName,string subscriptionName)
        {
            Console.WriteLine($"Creating Subscription {topicName}/{subscriptionName}");
            return await m_administrationClient.CreateSubscriptionAsync(topicName, subscriptionName);     
        }

        public async Task<SubscriptionProperties> CreateSubscriptionWithSqlFilter(string topicName,string subscriptionName, string sqlExpression)
        {
            Console.WriteLine($"Create Subscription with sql filter {topicName}/{subscriptionName} ({sqlExpression})");

            var subscriptionOption = new CreateSubscriptionOptions(topicName, subscriptionName);

            var ruleOption = new CreateRuleOptions("Default",new SqlRuleFilter(sqlExpression));

            return await m_administrationClient.CreateSubscriptionAsync(subscriptionOption,ruleOption);
        }

        public async Task<SubscriptionProperties> CreateSubscriptionWithCorrelationFilter(string topicName,string subscriptionName,string correlationId)
        {
            Console.WriteLine($"Create subscription with correlation Filter ..");

            var  subscriptionOption = new CreateSubscriptionOptions(topicName, subscriptionName);

            var ruleOption = new CreateRuleOptions("Default", new CorrelationRuleFilter(correlationId));

            return await m_administrationClient.CreateSubscriptionAsync(subscriptionOption, ruleOption);
        }

        public async Task<List<string>> GetSubscriptionsForTopic(string topicName)
        {
            var subscriptions = new List<string>();

            try
            {
                await foreach (var subscription in m_administrationClient.GetSubscriptionsAsync(topicName))
                {
                    subscriptions.Add(subscription.SubscriptionName);
                }
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
                // Handle the case when the topic doesn't exist
                Console.WriteLine($"Topic '{topicName}' not found.");
            }

            return subscriptions;
        }


    }

}
