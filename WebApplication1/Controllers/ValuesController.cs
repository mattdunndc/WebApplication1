using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using CsvHelper;
using WebApplication1.Models;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Razor.Text;

namespace WebApplication1.Controllers
{
    //[Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        //
        [Route("api/test")]
        [HttpGet]
        public string GetFin()
        {
            //string test = "yes";
            var fname = "fin171";
            var path = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(@"~/App_Data/{0}.csv", fname));
            string url = @"http://finviz.com/login_submit.ashx";
            string urlexport =
            //@"http://elite.finviz.com/export.ashx?v=111&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector";
            //@"http://elite.finviz.com/export.ashx?v=151&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector";
            @"http://elite.finviz.com/export.ashx?v=171&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector";
            //@"http://elite.finviz.com/export.ashx?v=150&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector";
            CookieContainer cc = new CookieContainer();
            HttpWebRequest http = WebRequest.Create(url) as HttpWebRequest;
            http.KeepAlive = true;
            http.Method = "POST";
            http.ContentType = "application/x-www-form-urlencoded";
            //
            Uri target = new Uri("http://finviz.com/");
            string cols = HttpContext.Current.Server.UrlEncode("0,1,2,3,4,5,7,14,28,29,42,43,44,46,52,53,54,57,58,59,65,66,67");
            string f = HttpContext.Current.Server.UrlEncode("GA1.2.1218077229.1483581984");
            string g = HttpContext.Current.Server.UrlEncode("screener.ashx?v=151&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector");

            cc.Add(new Cookie("customTable", cols) { Domain = target.Host });
            //_ga=GA1.2.1218077229.1483581984
            cc.Add(new Cookie("screenerUrl", g) { Domain = target.Host });
            //screener.ashx?v=151&f=sh_avgvol_o500,sh_instown_o70,sh_price_o5,ta_rsi_os40,targetprice_a20&ft=3&o=sector
            cc.Add(new Cookie("_ga", f) { Domain = target.Host });
            //
            http.CookieContainer = cc;

            string postData = "email=mattdunndc%40gmail.com&password=Be0nthew%40tch&remember=true";
            byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(postData);
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
        // GET api/values
        //public IEnumerable<string> Get()
        [Route("api/finviz/{filename?}")]
        [HttpGet]
        public IEnumerable<FinvizQuote> GetFinviz(string filename = "")
        {
            IEnumerable<FinvizQuote> quotes = new List<FinvizQuote>();
            var path = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(@"~/App_Data/{0}.csv", filename));
            using (TextReader tr = File.OpenText(path))
            {
                var csv = new CsvReader(tr);
                csv.Configuration.RegisterClassMap<FinvizQuoteMap>();
                quotes = csv.GetRecords<FinvizQuote>().ToList();

            }//tr

            return quotes;
        }
        // GET api/values
        //public IEnumerable<string> Get()
        [Route("api/values/{tickers?}")]
        [HttpGet]
        public IEnumerable<YahooQuote> Get(string tickers="")
        {
            IEnumerable<YahooQuote> quotes = new List<YahooQuote>();
            //string tickers = "WDAY, OAK, MAC";
            //Removes white space, converts to uppercase & splits symbols
            string[] symbols = Helpers.SplitTickers(tickers);

            DataTable dt = new DataTable("test");
           
            foreach (string symbol in symbols)
            {
                //Constructs Yahoo's URL to request data from
                //string path = Path.Combine(folder, symbol + ".csv");
                var path = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(@"~/App_Data/{0}.csv",symbol));
                var path2 = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(@"~/App_Data/{0}_rsi.csv", symbol));

                var interval = "d";
                var nMonth = (DateTime.Now.Month - 1).ToString();
                var nDay = DateTime.Now.Day.ToString();
                string folder = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
                //var url = string.Format(@"http://real-chart.finance.yahoo.com/table.csv?s={0}&a=0&b=1&c=2016&d=0&e=7&f2017&g={1}&ignore=.csv",symbol,interval);
                //var url = @"http://real-chart.finance.yahoo.com/table.csv?s=" + symbol + "&a=0" + "&b=1" +
                //    "&c=2016" + "&d=0" + "&e=1" + "&f=2017" + "&g=" + interval + "&ignore=.csv";
                //var url = @"http://real-chart.finance.yahoo.com/table.csv?s=" + symbol + "&a=0" + "&b=1" +
                //    "&c=2016" + "&g=" + interval + "&ignore=.csv";
                var url = @"http://real-chart.finance.yahoo.com/table.csv";
                string yUrl = String.Format("{0}?s={1}&a=0&b=1&c=2016&g={2}&ignore=.csv",url,symbol,interval);
                //string yUrl = @"http://elite.finviz.com/export.ashx?v=152&f=ind_stocksonly,sh_price_o5,ta_rsi_os40&ft=4";
                try
                {
                    Helpers.DownloadSymbolsToCSV(yUrl, path, folder, symbol);

                    using (TextReader tr = File.OpenText(path))
                    {
                        var csv = new CsvReader(tr);
                        quotes = csv.GetRecords<YahooQuote>().OrderBy(o => o.Date).ToList();
                    }

                    //using (var sw = new StreamWriter(path2))
                    //{
                    //    var writer = new CsvWriter(sw);
                    //    {
                    //        var csv = new CsvReader(tr);
                    //        quotes = csv.GetRecords<YahooQuote>().OrderBy(o => o.Date).ToList();
                    //        decimal prevClose = 0;
                    //        decimal change=0;
                    //        decimal gain=0;
                    //        decimal loss = 0;
                    //        decimal avgGain=0;
                    //        decimal avgLoss = 0;
                    //        decimal prevAvgGain = 0;
                    //        decimal prevAvgLoss = 0;
                    //        decimal rsi = 0;
                    //        decimal rs = 0;
                    //        DateTime dshort = DateTime.Now;
                    //        int i = 0;
                    //        writer.WriteHeader<YahooQuote>();    //write header
                    //        foreach (var q in quotes)
                    //        {
                    //            i++;
                    //            dshort = q.Date;
                    //            change = (i > 1) ? Decimal.Subtract(q.Close,prevClose) : 0; // don't calc first time through
                    //            prevClose = q.Close;     
                    //            gain = (change > 0) ? change : 0;
                    //            loss = (change < 0) ? Math.Abs(change) : 0;
                    //            if (i > 1 && i < 16)
                    //            {
                    //                avgGain = avgGain + gain;
                    //                avgLoss = avgLoss + loss;
                    //            }
                    //            if (i == 15)
                    //            {
                    //                avgGain = avgGain/14;
                    //                avgLoss = avgLoss/14;
                    //                prevAvgGain = avgGain;
                    //                prevAvgLoss = avgLoss;
                    //            }
                    //            else if (i > 15)
                    //            {
                    //                avgGain = (prevAvgGain*13 + gain)/14;
                    //                avgLoss = (prevAvgLoss*13 + loss)/14;
                    //                prevAvgGain = avgGain;
                    //                prevAvgLoss = avgLoss;
                    //            }
                    //            //write record field by field
                    //            writer.WriteField(q.Ticker);
                    //            writer.WriteField(q.Date.ToShortDateString());
                    //            writer.WriteField(q.Open);
                    //            writer.WriteField(q.High);
                    //            writer.WriteField(q.Low);
                    //            writer.WriteField(q.Close);
                    //            writer.WriteField(q.Volume);
                    //            writer.WriteField(q.Adj_Close);
                    //            writer.WriteField(change);
                    //            writer.WriteField(gain);
                    //            writer.WriteField(loss);
                    //            if (i > 14)  //starting i15 is row 16 in excel
                    //            {
                    //                rs = avgGain/avgLoss;
                    //                rsi = (avgLoss == 0) ? 100 : 100 - (100/(1 + rs));
                    //                writer.WriteField(avgGain);
                    //                writer.WriteField(avgLoss);
                    //                writer.WriteField(rs); //RS
                    //                writer.WriteField(rsi); //=IF(avgLoss=0,100,100-(100/(1+RS)))
                    //            }

                    //            //ensure you write end of record when you are using WriteField method
                    //            writer.NextRecord();
                    //            //
                    //        }
                    //    }
                    //} //writer
                } //try
                catch
                {
                    //MessageBox.Show("Could not locate " + symbol);
                }
            }
            return quotes;  //new string[] { "value1", "value2" };
        }

        // GET api/values
        //public IEnumerable<string> Get()
        [Route("api/rsi/{tickers?}")]
        [HttpGet]
        public IEnumerable<YahooQuote> GetRsi(string tickers = "")
        {
            IEnumerable<YahooQuote> quotes = new List<YahooQuote>();
            //string tickers = "WDAY, OAK, MAC";
            //Removes white space, converts to uppercase & splits symbols
            string[] symbols = Helpers.SplitTickers(tickers);
            
            foreach (string symbol in symbols)
            {
                //Constructs Yahoo's URL to request data from
                //string path = Path.Combine(folder, symbol + ".csv");
                var path = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(@"~/App_Data/{0}.csv", symbol));
                var path2 = System.Web.Hosting.HostingEnvironment.MapPath(string.Format(@"~/App_Data/{0}_rsi.csv", symbol));

                var interval = "d";
                var nMonth = (DateTime.Now.Month - 1).ToString();
                var nDay = DateTime.Now.Day.ToString();
                string folder = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
                //var url = string.Format(@"http://real-chart.finance.yahoo.com/table.csv?s={0}&a=0&b=1&c=2016&d=0&e=7&f2017&g={1}&ignore=.csv",symbol,interval);
                //var url = @"http://real-chart.finance.yahoo.com/table.csv?s=" + symbol + "&a=0" + "&b=1" +
                //    "&c=2016" + "&d=0" + "&e=1" + "&f=2017" + "&g=" + interval + "&ignore=.csv";
                //var url = @"http://real-chart.finance.yahoo.com/table.csv?s=" + symbol + "&a=0" + "&b=1" +
                //    "&c=2016" + "&g=" + interval + "&ignore=.csv";
                var url = @"http://real-chart.finance.yahoo.com/table.csv";
                string yUrl = String.Format("{0}?s={1}&a=0&b=1&c=2016&g={2}&ignore=.csv", url, symbol, interval);
                //string yUrl = @"http://elite.finviz.com/export.ashx?v=152&f=ind_stocksonly,sh_price_o5,ta_rsi_os40&ft=4";
                try
                {
                    Helpers.DownloadSymbolsToCSV(yUrl, path, folder, symbol);

                    using (TextReader tr = File.OpenText(path))
                    {
                        var csv = new CsvReader(tr);
                        quotes = csv.GetRecords<YahooQuote>().OrderBy(o => o.Date).ToList();
                        //
                        using (var sw = new StreamWriter(path2))
                        {
                            var writer = new CsvWriter(sw);
                            //
                                decimal prevClose = 0;
                                decimal change = 0;
                                decimal gain = 0;
                                decimal loss = 0;
                                decimal avgGain = 0;
                                decimal avgLoss = 0;
                                decimal prevAvgGain = 0;
                                decimal prevAvgLoss = 0;
                                decimal rsi = 0;
                                decimal rs = 0;
                                decimal ema12 = 0;
                                decimal ema26 = 0;
                                decimal prevEMA12 = 0;
                                decimal prevEMA26 = 0;
                                decimal macd = 0;

                                decimal emaMulti = Decimal.Divide(2,13);
                                decimal emaMulti2 = Decimal.Divide(2, 27);
                                decimal macdMulti = Decimal.Divide(2, 10);

                                decimal signal = 0;
                                decimal avgSig = 0;
                                decimal prevAvgSig = 0;
                                DateTime dshort = DateTime.Now;
                                int i = 0;
                                writer.WriteHeader<YahooQuote>();    //write header
                                foreach (var q in quotes)
                                {
                                    i++;
                                    dshort = q.Date;
                                    change = (i > 1) ? Decimal.Subtract(q.Close, prevClose) : 0; // don't calc first time through
                                    prevClose = q.Close;
                                    gain = (change > 0) ? change : 0;
                                    loss = (change < 0) ? Math.Abs(change) : 0;
                                    if (i < 13)
                                        ema12 = ema12 + q.Close; //count first one on row 2
                                    if (i < 27)
                                        ema26 = ema26 + q.Close;
                                    
                                    if (i > 1 && i < 16)
                                    {
                                        avgGain = avgGain + gain;
                                        avgLoss = avgLoss + loss;
                                    }
                                    //ema12 calc
                                    if (i == 12)
                                    {
                                        ema12 = ema12/12;
                                        prevEMA12 = ema12;
                                    }
                                    else if (i > 12)
                                    { //= Close * (2 / (12 + 1)) + ema12 * (1 - (2 / (12 + 1)))
                                        ema12 = (q.Close - prevEMA12 ) * emaMulti + prevEMA12 ;
                                        prevEMA12 = ema12;
                                    }
                                    //ema26 calc
                                    if (i == 26)
                                    {
                                        ema26 = ema26 / 26;
                                        prevEMA26 = ema26;
                                    }
                                    else if (i > 26)
                                    { //= Close * (2 / (12 + 1)) + ema12 * (1 - (2 / (12 + 1)))
                                        ema26 = (q.Close - prevEMA26) * emaMulti2 + prevEMA26;
                                        prevEMA26 = ema26;
                                    }
                                //macd calc
                                //macd signal calc
                                macd = decimal.Subtract(ema12, ema26);

                                //accum MACD for signal 9 day
                                if (i > 25 && i < 35) //row 27 to 35
                                {
                                        signal = signal + macd; //decimal.Subtract(ema12, ema26);
                                        if (i == 34) //excel row 35
                                        {
                                            avgSig = decimal.Divide(signal, 9);
                                            prevAvgSig = avgSig;
                                        }
                                    }
                                    else if (i > 34)
                                    {
                                        avgSig = (macd-prevAvgSig) *  macdMulti + prevAvgSig;
                                        prevAvgSig = avgSig;
                                    }

                                    if (i == 15)
                                        {
                                            avgGain = avgGain/14;
                                            avgLoss = avgLoss/14;
                                            prevAvgGain = avgGain;
                                            prevAvgLoss = avgLoss;
                                        }
                                        else if (i > 15)
                                        {
                                            avgGain = (prevAvgGain*13 + gain)/14;
                                            avgLoss = (prevAvgLoss*13 + loss)/14;
                                            prevAvgGain = avgGain;
                                            prevAvgLoss = avgLoss;
                                        }
                                    //write record field by field
                                    writer.WriteField(q.Ticker);
                                    writer.WriteField(q.Date.ToShortDateString());
                                    writer.WriteField(q.Open);
                                    writer.WriteField(q.High);
                                    writer.WriteField(q.Low);
                                    writer.WriteField(q.Close);
                                    writer.WriteField(q.Volume);
                                    writer.WriteField(q.Adj_Close);
                                    writer.WriteField(change);
                                    writer.WriteField(gain);
                                    writer.WriteField(loss);
                                    if (i > 11) //i12 is row 13
                                    {
                                        if (i > 14)
                                        {
                                            rs = avgGain/avgLoss;
                                            rsi = (avgLoss == 0) ? 100 : 100 - (100/(1 + rs));
                                            writer.WriteField(avgGain);
                                            writer.WriteField(avgLoss);
                                            writer.WriteField(rs); //RS
                                            writer.WriteField(rsi); //=IF(avgLoss=0,100,100-(100/(1+RS)))
                                        }
                                        else
                                        {
                                            writer.WriteField("");
                                            writer.WriteField("");
                                            writer.WriteField(""); //RS
                                            writer.WriteField(""); //=IF(avgLoss=0,100,100-(100/(1+RS)))
                                        }
                                        writer.WriteField(ema12);  //= Close * (2 / (12 + 1)) + ema12 * (1 - (2 / (12 + 1)))
                                        if (i > 25)
                                        {
                                            writer.WriteField(ema26);
                                            writer.WriteField(macd);  //MACD
                                            if (i >= 34) //excel line 35
                                            {
                                                writer.WriteField(avgSig);
                                                writer.WriteField(macd - avgSig);
                                            }
                                            else
                                            {
                                                writer.WriteField("");
                                                writer.WriteField("");
                                            }
                                        }
                                        else
                                        {
                                            writer.WriteField("");
                                            writer.WriteField("");
                                            writer.WriteField("");
                                            writer.WriteField("");
                                            writer.WriteField("");
                                    }

                                    }
                                    
                                    //ensure you write end of record when you are using WriteField method
                                    writer.NextRecord();
                                    //
                                    //
                                }// foreach q in quotes
                            }// using streamwriter
                    }//useing textreader

                   
                } //try
                catch
                {
                    //MessageBox.Show("Could not locate " + symbol);
                }
            }
            return quotes;  //new string[] { "value1", "value2" };
        }
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
