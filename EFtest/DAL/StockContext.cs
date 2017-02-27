using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using EFtest.Models;

namespace EFtest.DAL
{
    public class StockContext : DbContext
    {
        public StockContext() : base("StockContext")
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockQuote> StockQuotes { get; set; }
       

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
