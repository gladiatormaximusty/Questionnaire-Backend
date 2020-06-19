namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUserByForgetPwd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpUsers", "EmailVerifyStr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AbpUsers", "EmailVerifyStr");
        }
    }
}
