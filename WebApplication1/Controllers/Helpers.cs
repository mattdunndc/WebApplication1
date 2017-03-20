using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

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

        public static string MergeFin()
        {
            var fname = "f151-" + (DateTime.Now.ToString(@"yyyy-MM-dd"));
            var path1 = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(@"~/App_Data/{0}.csv", fname));
            fname = "f171-" + (DateTime.Now.ToString(@"yyyy-MM-dd"));
            var path2 = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(@"~/App_Data/{0}.csv", fname));
            var first = File.ReadAllLines(path1);
            var second = File.ReadAllLines(path2);
            fname = "f000-" + (DateTime.Now.ToString(@"yyyy-MM-dd"));
            var path3 = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(@"~/App_Data/{0}.csv", fname));
            var result = first.Zip(second, (f, s) => string.Join(",", f, s));
            File.WriteAllLines(path3, result);

            return "OK";
        }
        public static string WriteDailyCSV(string viewnumber)
        {
            //string test = "yes";
            var fname = "f" + viewnumber + "-" + (DateTime.Now.ToString(@"yyyy-MM-dd"));
            var path = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(@"~/App_Data/{0}.csv", fname));
            string url = @"http://finviz.com/login_submit.ashx";
            string urlexport = String.Format(@"http://elite.finviz.com/export.ashx?v={0}&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector", viewnumber);
            //@"http://elite.finviz.com/export.ashx?v=171&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector";
            //@"http://elite.finviz.com/export.ashx?v=150&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector";
            //  http://elite.finviz.com/screener.ashx?v=152&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=4&o=-dividendyield
            CookieContainer cc = new CookieContainer();
            HttpWebRequest http = WebRequest.Create(url) as HttpWebRequest;
            http.KeepAlive = true;
            http.Method = "POST";
            http.ContentType = "application/x-www-form-urlencoded";
            //
            Uri target = new Uri("http://finviz.com/");
            string cols = HttpContext.Current.Server.UrlEncode("0,1,2,3,4,5,14,28,44,46,52,54,57,58,59,65,66,67,69");
            string f = HttpContext.Current.Server.UrlEncode("GA1.2.1218077229.1483581984");
            string g = HttpContext.Current.Server.UrlEncode("screener.ashx?v=151&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector");
            string h = HttpContext.Current.Server.UrlEncode("1");

            //http://elite.finviz.com/export.ashx?v=151&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector
            cc.Add(new Cookie("customTable", cols) { Domain = target.Host });
            //_ga=GA1.2.1218077229.1483581984
            cc.Add(new Cookie("screenerUrl", g) { Domain = target.Host });
            //screener.ashx?v=151&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector
            //cc.Add(new Cookie("_ga", f) { Domain = target.Host });
            //cc.Add(new Cookie("_gat", h) { Domain = target.Host });
            cc.Add(new Cookie("domain", ".finviz.com") { Domain = target.Host });
            //domain=.finviz.com
            //_gat=1
            http.CookieContainer = cc;

            string postData = @"email=mattdunndc%40gmail.com&password=Be0nthew%40tch&remember=true";
            byte[] dataBytes = Encoding.UTF8.GetBytes(postData);
            http.ContentLength = dataBytes.Length;
            using (Stream postStream = http.GetRequestStream())
            {
                postStream.Write(dataBytes, 0, dataBytes.Length);
            }
            HttpWebResponse httpResponse = http.GetResponse() as HttpWebResponse;
            // Probably want to inspect the http.Headers here first
            http = WebRequest.Create(urlexport) as HttpWebRequest;
            //
            http.CookieContainer = cc;

            HttpWebResponse httpResponse2 = http.GetResponse() as HttpWebResponse;
            byte[] data = new System.IO.BinaryReader(httpResponse2.GetResponseStream()).ReadBytes((int)httpResponse2.ContentLength);
            System.IO.File.WriteAllBytes(path, data);
            return httpResponse2.StatusDescription;
        }
    }
}
