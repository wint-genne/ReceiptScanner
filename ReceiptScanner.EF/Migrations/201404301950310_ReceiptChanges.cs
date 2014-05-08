namespace ReceiptScanner.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReceiptChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Receipts", "ShopName", c => c.String());
            AddColumn("dbo.Receipts", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Receipts", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Receipts", "AccountNumber", c => c.String());
            DropColumn("dbo.Receipts", "Shop");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Receipts", "Shop", c => c.String());
            DropColumn("dbo.Receipts", "AccountNumber");
            DropColumn("dbo.Receipts", "Total");
            DropColumn("dbo.Receipts", "Date");
            DropColumn("dbo.Receipts", "ShopName");
        }
    }
}
