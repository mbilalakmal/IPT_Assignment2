using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k173669_Q4
{
    public class MergeProcess
    {
        private readonly MergeConfig config;
        private readonly ILogger<MergeProcess> logger;

        public MergeProcess(IOptions<MergeConfig> config, ILogger<MergeProcess> logger)
        {
            this.config = config.Value;
            this.logger = logger;
        }

    }

    public class MergeConfig
    {
        public string InputDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public int IntervalBetween { get; set; }

    }
}
