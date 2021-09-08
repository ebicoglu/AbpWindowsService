using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AbpWindowsService.Data
{
    /* This is used if database provider does't define
     * IAbpWindowsServiceDbSchemaMigrator implementation.
     */
    public class NullAbpWindowsServiceDbSchemaMigrator : IAbpWindowsServiceDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}