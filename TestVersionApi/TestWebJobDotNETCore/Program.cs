using System;

namespace TestWebJobDotNETCore
{
    using System.IO;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.ServiceBus;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    using TestWebJob;

    using TestWebJobDotNETCore.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            var configurationRoot = BuildConfiguration();

            CreateSubscription(configurationRoot);

            Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configurationRoot.GetConnectionString("AzureWebJobsDashboard"));
            Environment.SetEnvironmentVariable("AzureWebJobsStorage", configurationRoot.GetConnectionString("AzureWebJobsStorage"));

            var configuration = new JobHostConfiguration();

            configuration.UseServiceBus(new ServiceBusConfiguration() { ConnectionString = configurationRoot.GetConnectionString("AzureWebJobsServiceBus") });

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection, configurationRoot);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            configuration.JobActivator = new JobActivator(serviceProvider);

            var host = new JobHost(configuration);

            host.RunAndBlock();
        }

        private static void CreateSubscription(IConfigurationRoot configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureWebJobsServiceBus");

            var nameSpace = NamespaceManager.CreateFromConnectionString(connectionString);

            var topicName = configuration["TopicName"];
            var version = configuration["Version"];
            var subscriptionName = configuration["SubscriptionName"];

            if (nameSpace.SubscriptionExists(topicName, subscriptionName) == false)
            {
                var filter = new SqlFilter($"Version = '{version}'");

                nameSpace.CreateSubscription(topicName, subscriptionName, filter);
            }
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection, IConfigurationRoot configurationRoot)
        {
            serviceCollection.AddDataProtection()
                .UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                })
                .SetApplicationName("testapp");

            serviceCollection.AddSingleton(configurationRoot);

            serviceCollection.AddScoped<Functions, Functions>();
        }
    }
}
