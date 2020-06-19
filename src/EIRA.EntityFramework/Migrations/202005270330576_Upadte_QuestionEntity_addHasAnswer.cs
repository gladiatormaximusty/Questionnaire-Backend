namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upadte_QuestionEntity_addHasAnswer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "HasAnswer", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Questions", "HasAnswer");
        }
    }
}
