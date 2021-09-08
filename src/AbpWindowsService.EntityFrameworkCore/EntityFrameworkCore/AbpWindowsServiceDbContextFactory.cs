using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AbpWindowsService.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class AbpWindowsServiceDbContextFactory : IDesignTimeDbContextFactory<AbpWindowsServiceDbContext>
    {
        public AbpWindowsServiceDbContext CreateDbContext(string[] args)
        {
            AbpWindowsServiceEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<AbpWindowsServiceDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new AbpWindowsServiceDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AbpWindowsService.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
