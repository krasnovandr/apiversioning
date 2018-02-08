namespace TestWebJobDotNETCore
{
    using System;

    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.DependencyInjection;

    public class JobActivator : IJobActivator
    {
        private readonly IServiceProvider _service;

        public JobActivator(IServiceProvider service)
        {
            _service = service;
        }

        public T CreateInstance<T>()
        {
            var service = _service.GetService<T>();
            return service;
        }
    }
}