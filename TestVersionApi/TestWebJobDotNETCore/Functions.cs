using System.IO;

using Microsoft.Azure.WebJobs;

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

        public async Task ProcessQueueMessage([ServiceBusTrigger("hellotopic", "subscriptionv1")] byte[] message, TextWriter log)
        {
            var dataProtector = this.dataProtectionProvider.CreateProtector("CreateProtector");

            var unprotect = dataProtector.Unprotect(message);

            log.WriteLine(Encoding.UTF8.GetString(unprotect));
        }
    }
}
