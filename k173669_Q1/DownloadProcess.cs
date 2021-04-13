using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace k173669_Q1
{
    public class DownloadProcess
    {
        private readonly HttpClient client;
        private readonly DownloadConfig config;
        private readonly ILogger<DownloadProcess> logger;

        public DownloadProcess(IOptions<DownloadConfig> config, ILogger<DownloadProcess> logger)
        {
            this.config = config.Value;
            this.logger = logger;

            client = new();
        }

        public async Task DownloadAndSave()
        {
            var url = config.URL;
            var outputDirectory = config.OutputDirectory;

            /// Validate URL and output directory
            bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)

                ;

            if (result == false)
            {
                /// TODO: Log instead of throwing exception
                throw new ApplicationException($"{url} is not a valid URL.");
            }

            if (Directory.Exists(outputDirectory) == false)
            {
                /// TODO: Log instead of throwing exception
                throw new ApplicationException($"{outputDirectory} is not an existing directory.");
            }

            string responseBody = await client.GetStringAsync(uriResult);

            logger.LogInformation($"Downloaded webpage at {DateTimeOffset.Now}.");

            string fileName = "Summary " + DateTime.Now.ToString("yyMMMdd hh.mm tt") + ".html";
            using StreamWriter outputFile = new(Path.Combine(outputDirectory, fileName));
            await outputFile.WriteAsync(responseBody);

            logger.LogInformation($"Saved webpage as" +
                $" {Path.Combine(outputDirectory, fileName)} at {DateTimeOffset.Now}.");
        }
    }

    public class DownloadConfig
    {
        public string URL { get; set; }
        public string OutputDirectory { get; set; }

        public int IntervalBetween { get; set; }
    }
}
