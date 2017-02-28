using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFtest.DAL;
using EFtest.Models;

namespace BasicWebwithEF.Controllers
{
    public class HomeController : Controller
    {
        private StockContext ctx = new StockContext();
        public ActionResult Index()
        {
            StockQuote sq = new StockQuote
            {
                StockQuoteID = 10,
                Open = 13.50M,
                Close = 14M
            };
            ctx.StockQuotes.Add(sq);
            ctx.SaveChanges();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            StockQuote stockquote = ctx.StockQuotes.Where(x => x.Close == 14).FirstOrDefault();

            return View(stockquote);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}