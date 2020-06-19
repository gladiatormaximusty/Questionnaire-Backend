namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUserByIsUpdateImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpUsers", "IsUpdateImage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AbpUsers", "IsUpdateImage");
        }
    }
}
