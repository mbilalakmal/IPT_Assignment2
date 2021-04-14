using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace k173669_Q4
{
    public class MergeWorker : BackgroundService
    {
        private readonly MergeProcess mergeProcess;
        private readonly MergeConfig config;

        public MergeWorker(MergeProcess archiveProcess, IOptions<MergeConfig> config)
        {
            this.mergeProcess = archiveProcess;
            this.config = config.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                mergeProcess.MergeXmlFilesToJson();
                await Task.Delay(config.IntervalBetween * 1000, stoppingToken);
            }
        }
    }
}
