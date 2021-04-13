using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace k173669_Q1
{
    public class DownloadWorker : BackgroundService
    {
        private readonly ILogger<DownloadWorker> _logger;
        private readonly DownloadProcess downloadProcess;
        private readonly DownloadConfig config;

        public DownloadWorker
            (
            ILogger<DownloadWorker> logger,
            DownloadProcess downloadProcess,
            IOptions<DownloadConfig> config
            )
        {
            _logger = logger;
            this.downloadProcess = downloadProcess;
            this.config = config.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await downloadProcess.DownloadAndSave();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(config.IntervalBetween * 1000, stoppingToken);
            }
        }
    }
}
