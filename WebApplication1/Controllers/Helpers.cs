using System.IO;
using System.Net;

namespace WebApplication1.Controllers
{
    public static class Helpers
    {
        public static void DownloadSymbolsToCSV(string url, string path, string folder, string symbol)
        {
            using (WebClient Client = new WebClient())
            {
                //Download .csv file from Yahoo
                Client.DownloadFile(url, path);

                //Create temp file
                string tempFile = Path.Combine(folder, symbol + "_temp.csv");
                using (var writer = new StreamWriter(tempFile))
                using (var reader = new StreamReader(File.OpenRead(path)))
                {
                    //Prepend Ticker to Header
                    string header = reader.ReadLine();
                    header = header.Insert(0, "Ticker,");
                    //Add _ to Adj Close header
                    header = header.Replace("Adj Close", "Adj_Close,Change,Gain,Loss,AvgGain,AvgLoss,RS,RSI,EMA12,EMA26,MACD,Signal,Histogram");
                    writer.WriteLine(header);

                    //Prepend ticker symbol to each line of quote information
                    while (!reader.EndOfStream)
                    {
                        string tickerInfo = reader.ReadLine();
                        tickerInfo = tickerInfo.Insert(0, symbol + ",");
                        var end = tickerInfo.Length;
                        tickerInfo = tickerInfo.Insert(end, ",0,0,0,0,0,0,0,0,0,0,0,0");
                        writer.WriteLine(tickerInfo);
                    }
                }
                File.Copy(tempFile, path, true);
                File.Delete(tempFile);
            }
        }

        public static string[] SplitTickers(string tickers)
        {
            tickers = tickers.Replace(" ", string.Empty);
            tickers = tickers.ToUpper();
            return tickers.Split(',');
        }
    }
}
