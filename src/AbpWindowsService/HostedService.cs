using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace AbpWindowsService
{
    public class HostedService : IHostedService
    {
        private readonly IAbpApplicationWithExternalServiceProvider _application;
        private readonly IServiceProvider _serviceProvider;
        private readonly MyDomainService _myDomainService;

        public HostedService(
            IAbpApplicationWithExternalServiceProvider application,
            IServiceProvider serviceProvider,
            MyDomainService myDomainService)
        {
            _application = application;
            _serviceProvider = serviceProvider;
            _myDomainService = myDomainService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _application.Initialize(_serviceProvider);

            _myDomainService.StartService();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _myDomainService.StopService();
            
            _application.Shutdown();

            return Task.CompletedTask;
        }
    }
}
