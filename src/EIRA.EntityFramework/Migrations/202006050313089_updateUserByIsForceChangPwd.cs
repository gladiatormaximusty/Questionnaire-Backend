namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUserByIsForceChangPwd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpUsers", "IsForceChangPwd", c => c.Boolean(nullable: false));
            DropColumn("dbo.AbpUsers", "EmailVerifyStr");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AbpUsers", "EmailVerifyStr", c => c.String());
            DropColumn("dbo.AbpUsers", "IsForceChangPwd");
        }
    }
}
