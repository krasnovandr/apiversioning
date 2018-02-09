using System.Configuration;
using System.IO;

using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;

namespace TestWebJob
{
    using System.Text;

    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
    using Microsoft.Extensions.DependencyInjection;

    public class Functions
    {
        private static readonly ServiceProvider ServiceProvider;

        static Functions()
        {
            var serviceCollection = new ServiceCollection();

            var storageConnection = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnection);

            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(ConfigurationManager.AppSettings["ContainerName"]);

            serviceCollection.AddDataProtection()
                .PersistKeysToAzureBlobStorage(container, "key.xml")
                .UseCryptographicAlgorithms(
                    new AuthenticatedEncryptorConfiguration
                    {
                        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                    }).SetApplicationName(ConfigurationManager.AppSettings["ApplicationName"]);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([ServiceBusTrigger("%TopicName%", "%SubscriptionName%")] byte[] message, TextWriter log)
        {
            var dataProtectionProvider = ServiceProvider.GetService<IDataProtectionProvider>();

            var dataProtector = dataProtectionProvider.CreateProtector(ConfigurationManager.AppSettings["PurporseString"]);

            var unprotect = dataProtector.Unprotect(message);

            log.WriteLine(Encoding.UTF8.GetString(unprotect));
        }
    }
}
