using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;

namespace WebApplication1.Models
{
    public class FinvizQuote
    {
        public int Number { get; set; }
        public string Ticker { get; set; }
        public string Company { get; set; }
        public string Sector { get; set; }
        //public DateTime Date { get; set; }
        //public decimal Open { get; set; }
        //public decimal High { get; set; }
        //public decimal Low { get; set; }
        //public decimal Close { get; set; }
        //public int Volume { get; set; }
        public decimal RSI { get; set; }
    }
}