using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class YahooQuote
    {
        public string Ticker { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
        public decimal Adj_Close { get; set; }


        public decimal Change { get; set; }
        public decimal Gain { get; set; }
        public decimal Loss { get; set; }
        public decimal AvgGain { get; set; }
        public decimal AvgLoss { get; set; }
        public decimal RS { get; set; }
        public decimal RSI { get; set; }

        public decimal EMA12 { get; set; }
        
        public decimal EMA26 { get; set; }
        
        public decimal MACD { get; set; }
        public decimal Signal { get; set; }
        public decimal Histogram { get; set; }


    }
}