using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace k173669_Q1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using Process process = new();
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.StartInfo.FileName = @"C:\Users\bilal\source\repos\IPT_Assignment1\k173669_Q1\bin\Release\netcoreapp3.1\k173669_Q1.exe";
                    process.StartInfo.Arguments = "https://www.psx.com.pk/market-summary/ C:\\Users\\bilal\\Downloads";

                    process.Start();
                }
                catch (Exception e)
                {

                    /// TODO: Log the exception
                    Console.WriteLine(e.Message);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
