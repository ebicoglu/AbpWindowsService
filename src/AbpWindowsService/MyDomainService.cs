using System.IO;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Serilog;
using Volo.Abp.DependencyInjection;

namespace AbpWindowsService
{
    public class MyDomainService : ITransientDependency
    {
        private readonly ILogger<MyDomainService> _logger;

        public MyDomainService(ILogger<MyDomainService> logger)
        {
            _logger = logger;
        }

        public void StartService()
        {
            _logger.LogError("Service started");
        }

        public void StopService()
        {
           _logger.LogError("Service stopped.");
        }
    }
}
