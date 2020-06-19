namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserEntityBu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpUsers", "BU_Id", c => c.Int());
            CreateIndex("dbo.AbpUsers", "BU_Id");
            AddForeignKey("dbo.AbpUsers", "BU_Id", "dbo.BUs", "Id");
            DropColumn("dbo.AbpUsers", "BUId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AbpUsers", "BUId", c => c.Long());
            DropForeignKey("dbo.AbpUsers", "BU_Id", "dbo.BUs");
            DropIndex("dbo.AbpUsers", new[] { "BU_Id" });
            DropColumn("dbo.AbpUsers", "BU_Id");
        }
    }
}
