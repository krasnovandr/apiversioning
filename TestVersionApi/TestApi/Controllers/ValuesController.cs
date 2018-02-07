using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
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

        public ValuesController(IOptions<MainSettings> settings)
        {
            _settings = settings.Value;
        }

        // GET api/values
        [HttpGet]
        public async Task<string> Get()
        {
            var sendClient = new TopicClient(_settings.ServiceBusSettings.ConnectionString, _settings.ServiceBusSettings.TopicName);

            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_settings.MessageToPush)));
            message.UserProperties.Add("Version", "V1");

            await sendClient.SendAsync(message);

            return _settings.MessageToPush;
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
