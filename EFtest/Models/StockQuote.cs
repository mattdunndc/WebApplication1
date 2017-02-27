using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFtest.Models
{
    public class StockQuote
    {
        public int ID { get; set; }
        public int StockQuoteID { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
        public decimal Adj_Close { get; set; }

        public virtual Stock Stock { get; set; }

    }
}
