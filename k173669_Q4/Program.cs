using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace k173669_Q4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string logPath = args.Length > 0 ? args[0] : @"C:\Users\bilal\Downloads\IPT_Assignment2\";

            /// Validate the  argument as an exisiting directory path.
            //string directoryPath = logPath;
            //if (Directory.Exists(directoryPath) == false)
            //throw new ApplicationException($"{logPath} is not an existing directory.");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(logPath, "LogFile_A2_Q4.txt"))
                .CreateLogger()
                ;

            try
            {
                Log.Information("Starting MergeWorker.");
                CreateHostBuilder(args).Build().Run();

                return;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Could not start MergeWorker.");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<MergeConfig>(hostContext.Configuration.GetSection("AppSettings"));
                    services.AddLogging();
                    services.AddSingleton<MergeProcess>();
                    services.AddHostedService<MergeWorker>();
                });
        }
    }
}
