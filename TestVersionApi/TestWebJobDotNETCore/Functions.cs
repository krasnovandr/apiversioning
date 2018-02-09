using System.IO;

using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;

namespace TestWebJob
{
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Configuration;

    public class Functions
    {
        private readonly IConfigurationRoot configurationRoot;

        private readonly IDataProtectionProvider dataProtectionProvider;

        public Functions(IConfigurationRoot configurationRoot, IDataProtectionProvider dataProtectionProvider)
        {
            this.configurationRoot = configurationRoot;
            this.dataProtectionProvider = dataProtectionProvider;
        }

        public void ProcessQueueMessage([ServiceBusTrigger("hellotopic", "subscriptionv1")] byte[] message, TextWriter log)
        {
           var dataProtector = dataProtectionProvider.CreateProtector("test");

            var unprotect = dataProtector.Unprotect(message);
            log.WriteLine(Encoding.UTF8.GetString(unprotect));
        }
    }
}
