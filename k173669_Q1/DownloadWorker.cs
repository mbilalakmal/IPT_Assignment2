using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace k173669_Q1
{
    public class DownloadWorker : BackgroundService
    {
        private readonly DownloadProcess downloadProcess;
        private readonly DownloadConfig config;

        public DownloadWorker(DownloadProcess downloadProcess, IOptions<DownloadConfig> config)
        {
            this.downloadProcess = downloadProcess;
            this.config = config.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await downloadProcess.DownloadAndSave();
                await Task.Delay(config.IntervalBetween * 1000, stoppingToken);
            }
        }
    }
}
