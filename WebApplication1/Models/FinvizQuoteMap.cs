using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;

namespace WebApplication1.Models
{
    public class FinvizQuoteMap : CsvClassMap<FinvizQuote>
    {
        public FinvizQuoteMap()
        {
            Map(m => m.Number).Name("No.");
            Map(m => m.Ticker).Name("Ticker");
            Map(m => m.Company).Name("Company");
            Map(m => m.Sector).Name("Sector");
            Map(m => m.Country).Name("Country");
            Map(m => m.Price).Name("Price").NameIndex(0);
            Map(m => m.PE).Name("P/E");
            Map(m => m.ATR).Name("Average True Range");
            Map(m => m.RSI).Name("Relative Strength Index (14)");

        }
    }
}