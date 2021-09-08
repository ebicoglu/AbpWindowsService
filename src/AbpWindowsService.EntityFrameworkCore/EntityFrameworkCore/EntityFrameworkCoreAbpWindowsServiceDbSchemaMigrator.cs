using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AbpWindowsService.Data;
using Volo.Abp.DependencyInjection;

namespace AbpWindowsService.EntityFrameworkCore
{
    public class EntityFrameworkCoreAbpWindowsServiceDbSchemaMigrator
        : IAbpWindowsServiceDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreAbpWindowsServiceDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the AbpWindowsServiceDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<AbpWindowsServiceDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
