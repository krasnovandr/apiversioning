using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace TestWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            config.UseServiceBus();

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();

            var nameSpace = NamespaceManager.CreateFromConnectionString(ConfigurationManager
                .ConnectionStrings["AzureWebJobsServiceBus"].ConnectionString);

            var filter = new TrueFilter();
            filter.Parameters.Add("df", "dfdf");

            nameSpace.CreateSubscription(
                ConfigurationManager.AppSettings["TopicName"],
                ConfigurationManager.AppSettings["SubscriptionName"],filter);
        }
    }
}
