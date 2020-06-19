namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpUsers", "Account", c => c.String(maxLength: 50));
            AddColumn("dbo.AbpUsers", "Image", c => c.String());
            AddColumn("dbo.AbpUsers", "BUId", c => c.Long());
            AddColumn("dbo.AbpUsers", "IsAdmin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AbpUsers", "IsAdmin");
            DropColumn("dbo.AbpUsers", "BUId");
            DropColumn("dbo.AbpUsers", "Image");
            DropColumn("dbo.AbpUsers", "Account");
        }
    }
}
