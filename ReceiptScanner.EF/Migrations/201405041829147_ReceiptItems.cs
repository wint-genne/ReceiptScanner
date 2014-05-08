namespace ReceiptScanner.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReceiptItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReceiptItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShopItem_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShopItems", t => t.ShopItem_Id)
                .Index(t => t.ShopItem_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReceiptItems", "ShopItem_Id", "dbo.ShopItems");
            DropIndex("dbo.ReceiptItems", new[] { "ShopItem_Id" });
            DropTable("dbo.ReceiptItems");
        }
    }
}
