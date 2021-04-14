using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

        public void MergeXmlFilesToJson()
        {

            /// Maintain history of each scrip in all categories
            Dictionary<Scrips, List<PriceSnapshot>> historicalStocks = new();

            /// Mirror subfolders from inputDir to ouputDir
            foreach (var subfolder in new DirectoryInfo(config.InputDirectory).GetDirectories())
            {
                var outputDir = Directory.CreateDirectory(
                    Path.Combine(config.OutputDirectory, subfolder.Name)
                    );

                var xmlFiles = subfolder.GetFiles()
                    .Where(file => file.Extension == ".xml")
                    .OrderBy(file => file.LastWriteTime)
                    .ToList();

                /// Each file contains a historic snapshot of scrips within that category
                foreach (var xmlFile in xmlFiles)
                {
                    var stocks = ReadFromXml(xmlFile.FullName);

                    var dateTime = xmlFile.LastWriteTime;

                    foreach (var stock in stocks)
                    {
                        if (historicalStocks.ContainsKey(stock) != true)
                        {
                            /// If a new scrip is encountered, initialize its List
                            historicalStocks[stock] = new List<PriceSnapshot>();
                        }
                        historicalStocks[stock].Add(
                            new PriceSnapshot() { Date=dateTime, Price=stock.Price}
                            );
                    }

                    logger.LogInformation(historicalStocks.Count.ToString());
                    logger.LogInformation(historicalStocks.First().Key.Scrip);

                    /// Delete the XML file
                    xmlFile.Delete();
                }
            }

            /// Store to JSON or merge if existing
            foreach (var historicalStock in historicalStocks)
            {
                var fileName = Path.Combine(
                        config.OutputDirectory,
                        historicalStock.Key.Category,
                        $"{historicalStock.Key.Scrip}.json"
                        );

                /// Check if JSON file already exists and read into memory if true
                if (File.Exists(fileName))
                {
                    var text = File.ReadAllText(fileName);
                    var historicalScrips = (HistoricalScrips)JsonSerializer.Deserialize(
                        text, typeof(HistoricalScrips)
                        );

                    historicalScrips.LastUpdatedOn = DateTime.Now;
                    historicalScrips.ScripData.AddRange(historicalStock.Value);

                    var serial = JsonSerializer.Serialize(historicalScrips);
                    File.WriteAllText(fileName, serial);
                }
                else
                {
                    var historicalScrips = new HistoricalScrips() {
                        Scrip = historicalStock.Key.Scrip,
                        LastUpdatedOn = DateTime.Now,
                        ScripData = historicalStock.Value,
                    };

                    var serial = JsonSerializer.Serialize(historicalScrips);
                    File.WriteAllText(fileName, serial);
                }
            }
        }

        private List<Scrips> ReadFromXml(string fileName)
        {
            XmlSerializer serializer = new(typeof(List<Scrips>));

            using FileStream fileStream = new(fileName, FileMode.Open);

            var stocks = (List<Scrips>)serializer.Deserialize(fileStream);

            return stocks;
        }

    }

    public class MergeConfig
    {
        public string InputDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public int IntervalBetween { get; set; }

    }
}
