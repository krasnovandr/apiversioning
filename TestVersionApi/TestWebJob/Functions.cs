using System.IO;

using Microsoft.Azure.WebJobs;

namespace TestWebJob
{
    using System.Text;

    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
    using Microsoft.Extensions.DependencyInjection;

    public class Functions
    {
        private static ServiceProvider serviceProvider;

        static Functions()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDataProtection()
                .UseCryptographicAlgorithms(
                    new AuthenticatedEncryptorConfiguration
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                        })
                        .SetApplicationName("testapp");

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([ServiceBusTrigger("%TopicName%", "%SubscriptionName%")] byte[] message, TextWriter log)
        {
            var dataProtectionProvider = serviceProvider.GetService<IDataProtectionProvider>();
            
            var dataProtector = dataProtectionProvider.CreateProtector("CreateProtector");

            var unprotect = dataProtector.Unprotect(message);

            log.WriteLine(Encoding.UTF8.GetString(unprotect));
        }
    }
}
