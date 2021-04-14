using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k173669_Q4
{
    public class HistoricalScrips
    {

        public string Scrip { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public List<PriceSnapshot> ScripData { get; set; } = new();
    }

    public struct PriceSnapshot
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
    }

}
