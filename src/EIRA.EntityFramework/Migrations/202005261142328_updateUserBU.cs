namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUserBU : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AbpUsers", "BU_Id", "dbo.BUs");
            DropIndex("dbo.AbpUsers", new[] { "BU_Id" });
            AlterColumn("dbo.AbpUsers", "BU_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.AbpUsers", "BU_Id");
            AddForeignKey("dbo.AbpUsers", "BU_Id", "dbo.BUs", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AbpUsers", "BU_Id", "dbo.BUs");
            DropIndex("dbo.AbpUsers", new[] { "BU_Id" });
            AlterColumn("dbo.AbpUsers", "BU_Id", c => c.Int());
            CreateIndex("dbo.AbpUsers", "BU_Id");
            AddForeignKey("dbo.AbpUsers", "BU_Id", "dbo.BUs", "Id");
        }
    }
}
