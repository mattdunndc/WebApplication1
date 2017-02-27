using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFtest.Models
{
    public class Stock
    {
        public int ID { get; set; }
        public string Ticker { get; set; }
        public string Company { get; set; }
        public string Sector { get; set; }
        public string Country { get; set; }
        public decimal InstitutionOwned { get; set; }
        public decimal Dividend { get; set; }
        public decimal RSI { get; set; }
        public DateTime CurrentDate { get; set; }

        public virtual ICollection<StockQuote> StockQuotes { get; set; }
    }
}
