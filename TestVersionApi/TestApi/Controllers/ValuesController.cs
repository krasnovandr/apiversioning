using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TestApi.Configuration;

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly MainSettings _settings;

        private readonly IDataProtector _protector;
        public ValuesController(IOptions<MainSettings> settings, IDataProtectionProvider provider)
        {
            _settings = settings.Value;
            _protector = provider.CreateProtector(GetType().FullName);
        }

        // GET api/values
        [HttpGet]
        public async Task<string> Get()
        {
            var sendClient = new TopicClient(_settings.ServiceBusSettings.ConnectionString, _settings.ServiceBusSettings.TopicName);
            var messageBody = string.Format(_settings.MessageToPush, _settings.Version);

            //var encryptedString = AesEncryption.EncryptString(messageBody, _settings.EncryptionKey);
            var encryptedValue = _protector.Protect(Encoding.UTF8.GetBytes(messageBody));
            var message = new Message(encryptedValue);
            message.UserProperties.Add("Version", _settings.Version);

            await sendClient.SendAsync(message);

            return messageBody;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
