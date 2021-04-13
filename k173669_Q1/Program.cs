using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Configuration;
using System.IO;

namespace k173669_Q1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string logPath = args.Length > 0 ? args[0] : @"C:\Users\bilal\Downloads\IPT_Assignment2\";

            /// Validate the  argument as an exisiting directory path.
            string directoryPath = logPath;
            if (Directory.Exists(directoryPath) == false)
                throw new ApplicationException($"{logPath} is not an existing directory.");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File( Path.Combine(logPath, "LogFile.txt"))
                .CreateLogger()
                ;

            CreateHostBuilder(args).Build().Run();

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<DownloadConfig>(hostContext.Configuration.GetSection("AppSettings"));
                    services.AddLogging();
                    services.AddSingleton<DownloadProcess>();
                    services.AddHostedService<DownloadWorker>();
                });
    }
}
