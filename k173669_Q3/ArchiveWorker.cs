using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace k173669_Q3
{
    public class ArchiveWorker : BackgroundService
    {
        private readonly ArchiveProcess archiveProcess;
        private readonly ArchiveConfig config;

        public ArchiveWorker(ArchiveProcess archiveProcess, IOptions<ArchiveConfig> config)
        {
            this.archiveProcess = archiveProcess;
            this.config = config.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                archiveProcess.ArchiveOldFiles();
                await Task.Delay(config.IntervalBetween * 1000, stoppingToken);
            }
        }
    }
}
