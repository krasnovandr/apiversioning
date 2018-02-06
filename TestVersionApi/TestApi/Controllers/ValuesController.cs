using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly string _topicName = Startup.Configuration["ServiceBusSettings:TopicName"];
        private readonly string _connectionString = Startup.Configuration["ServiceBusSettings:ConnectionString"];
        // GET api/values
        [HttpGet]
        public async Task<string> Get()
        {
            var messageBody = "Hello v2.0.0.0";
            var sendClient = new TopicClient(_connectionString, _topicName);

            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageBody)));

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
