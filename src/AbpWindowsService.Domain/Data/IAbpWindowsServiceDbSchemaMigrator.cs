using System.Threading.Tasks;

namespace AbpWindowsService.Data
{
    public interface IAbpWindowsServiceDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
