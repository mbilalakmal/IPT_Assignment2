using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k173669_Q3
{
    public class ArchiveProcess
    {
        private readonly ArchiveConfig config;
        private readonly ILogger<ArchiveProcess> logger;

        public ArchiveProcess(IOptions<ArchiveConfig> config, ILogger<ArchiveProcess> logger)
        {
            this.config = config.Value;
            this.logger = logger;
        }

        public void ArchiveOldFiles()
        {
            var inputDirectory = new DirectoryInfo(config.InputDirectory);

            /// Each subfolder is a category
            foreach (var subfolder in inputDirectory.GetDirectories())
            {
                /// Create mirror subfolder in outputDirectory
                var outputDirectory = Directory.CreateDirectory(
                    Path.Combine(config.OutputDirectory, subfolder.Name)
                    );

                logger.LogInformation($"Created subdirectory for {subfolder.Name}");

                /// Copy each xml file except the latest one
                var xmlFiles = subfolder.GetFiles()
                    .Where(file => file.Extension == ".xml")
                    .OrderByDescending(file => file.LastWriteTime).ToList();

                if(xmlFiles.Count < 2)
                {
                    /// Do nothing
                    logger.LogInformation("Not enough xml files to archive.");
                }
                else
                {
                    /// Don't copy the first one
                    xmlFiles.RemoveAt(0);
                    foreach (var file in xmlFiles)
                    {
                        file.MoveTo(Path.Combine(outputDirectory.FullName, file.Name));
                    }
                }
            }
            logger.LogInformation("Completed archiving process.");

        }
    }
    public class ArchiveConfig
    {
        public string InputDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public int IntervalBetween { get; set; }
    }
}
