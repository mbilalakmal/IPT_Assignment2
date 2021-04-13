using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace k173669_Q1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<DownloadConfig>(hostContext.Configuration.GetSection("AppSettings"));
                    services.AddLogging();
                    services.AddSingleton<DownloadProcess>();
                    services.AddHostedService<DownloadWorker>();
                });
    }
}
