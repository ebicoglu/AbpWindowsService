using AbpWindowsService.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace AbpWindowsService.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpWindowsServiceEntityFrameworkCoreModule)
        )]
    public class AbpWindowsServiceDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
