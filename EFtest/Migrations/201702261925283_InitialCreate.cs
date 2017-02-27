namespace EFtest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockQuote",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StockQuoteID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Open = c.Decimal(nullable: false, precision: 18, scale: 2),
                        High = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Low = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Close = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Volume = c.Int(nullable: false),
                        Adj_Close = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Stock_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Stock", t => t.Stock_ID)
                .Index(t => t.Stock_ID);
            
            CreateTable(
                "dbo.Stock",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Ticker = c.String(),
                        Company = c.String(),
                        Sector = c.String(),
                        Country = c.String(),
                        InstitutionOwned = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Dividend = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RSI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockQuote", "Stock_ID", "dbo.Stock");
            DropIndex("dbo.StockQuote", new[] { "Stock_ID" });
            DropTable("dbo.Stock");
            DropTable("dbo.StockQuote");
        }
    }
}
