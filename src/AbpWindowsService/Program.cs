using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace AbpWindowsService
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                CreateLogger();
                Log.Information("Starting application...");
                await CreateHostBuilder(args).Build().RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void CreateLogger()
        {
            var startupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Combine(startupPath, "Logs/Log.txt"); 
            //if static log file path is not given then c:\windows\system32\ will be your default working directory.

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File(file))
                .CreateLogger();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseAutofac()
                .UseSerilog()
                .UseWindowsService(x => x.ServiceName = "AbpWindowsService")
                .ConfigureAppConfiguration((context, config) =>
                {
                    //setup your additional configuration sources
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplication<AbpWindowsServiceModule>();
                });

    }
}
