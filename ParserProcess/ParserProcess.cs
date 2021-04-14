using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ParserLibrary
{
    public class ParserProcess
    {
        private readonly ParserConfig config;
        private readonly ILogger<ParserProcess> logger;

        public ParserProcess(IOptions<ParserConfig> config, ILogger<ParserProcess> logger)
        {
            this.config = config.Value;
            this.logger = logger;
        }

        public void ParseAndSaveAsXml()
        {
            /// TODO: validate Config values
            /// 

            var directory = new DirectoryInfo(config.InputDirectory);
            var latestFile = directory.GetFiles()
                .Where(file => file.Extension == ".html")
                .OrderByDescending(file => file.LastWriteTime)
                .FirstOrDefault();

            if(latestFile == default(FileInfo))
            {
                /// TODO: No HTML files found

                logger.LogError("No HTML files found.");
            }

            var stocks = ParseFromHtml(latestFile.FullName);

            logger.LogInformation("Parsed HTML file.");

            SaveAsXml(stocks);

            logger.LogInformation("Saved XML files.");
        }

        private Dictionary<string, List<Scrips>> ParseFromHtml(string filePath)
        {
            Dictionary<string, List<Scrips>> stocks = new();

            HtmlDocument document = new();
            document.Load(filePath);

            HtmlNode htmlNode = document.DocumentNode.SelectSingleNode("//body");

            // Select each table head for category name
            foreach (HtmlNode nNode in htmlNode.Descendants("th"))
            {
                if (nNode.NodeType != HtmlNodeType.Element) { continue; }

                string category = SanitizeInputString(nNode.InnerText);

                if (stocks.ContainsKey(category) != true)
                {
                    /// If a new cateogory is encountered, initialize its List
                    stocks[category] = new List<Scrips>();
                }

                // Reach table that includes all scrips and their values in this category
                HtmlNode pNode = nNode.ParentNode.ParentNode.ParentNode;

                var collection = pNode.SelectNodes(".//comment()/following-sibling::td").ToList();

                // Scrip names are 8 indices apart
                for (int idx = 0; idx < collection.Count; idx += 8)
                {
                    string companyName = SanitizeInputString(collection[idx].InnerText);
                    // Current price is 5 indices ahead of scrip name
                    var currentValue = collection[idx + 5];

                    if (double.TryParse(currentValue.InnerText, out double currentPrice) == false)
                    {
                        throw new ApplicationException($"Unable to parse current price of {companyName} as a double.");
                    }

                    Scrips stock = new Scrips
                    {
                        Category = category,
                        Price = currentPrice,
                        Scrip = companyName,
                    };

                    stocks[category].Add(stock);
                }
            }
            return stocks;
        }

        private void SaveAsXml(Dictionary<string, List<Scrips>> stocks)
        {
            /// Create time-stamped files inside category folders
            /// 


            XmlSerializer serializer = new XmlSerializer(typeof(List<Scrips>));

            foreach (var item in stocks)
            {
                string categoryName = item.Key;

                /// If folder not created, create it here
                
                Directory.CreateDirectory(Path.Combine(config.OutputDirectory, categoryName));
                

                string fileName = Path.Combine(
                    config.OutputDirectory, categoryName, $"{DateTime.Now.ToString("yyyyMMMdd hhmmtt")}.xml"
                );

                using FileStream fileStream = new FileStream(fileName, FileMode.Create);

                var scripsList = item.Value;
                serializer.Serialize(fileStream, scripsList);
            }
        }

        private string SanitizeInputString(string input)
        {
            // Remove / to prevent nested folders and trailing period to prevent undeletable files
            return input.Replace(@"/", "").Replace(@"\", "").Trim('.', ' ');
        }
    }

    public class ParserConfig
    {
        public string InputDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public int IntervalBetween { get; set; }
    }
}
