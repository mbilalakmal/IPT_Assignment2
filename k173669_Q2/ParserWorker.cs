using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ParserLibrary;
using System.Threading;
using System.Threading.Tasks;

namespace k173669_Q2
{
    public class ParserWorker : BackgroundService
    {
        private readonly ParserProcess parserProcess;
        private readonly ParserConfig config;

        public ParserWorker(ParserProcess parserProcess, IOptions<ParserConfig> config)
        {
            this.parserProcess = parserProcess;
            this.config = config.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                parserProcess.ParseAndSaveAsXml();
                await Task.Delay(config.IntervalBetween * 1000, stoppingToken);
            }
        }
    }
}
