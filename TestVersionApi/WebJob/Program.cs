using System.Configuration;

using Microsoft.Azure.WebJobs;

namespace WebJob
{
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();

            CreateSubscription();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
        
        private static void CreateSubscription()
        {
            var nameSpace = NamespaceManager.CreateFromConnectionString(ConfigurationManager
                .ConnectionStrings["AzureWebJobsServiceBus"].ConnectionString);

            var topicName = ConfigurationManager.AppSettings["TopicName"];
            var version = ConfigurationManager.AppSettings["Version"];
            var subscriptionName = ConfigurationManager.AppSettings["SubscriptionName"];

            if (nameSpace.SubscriptionExists(topicName, subscriptionName) == false)
            {
                var filter = new SqlFilter($"Version = '{version}'");

                nameSpace.CreateSubscription(topicName, subscriptionName, filter);
            }

        }
    }
}
