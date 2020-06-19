namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_QuestionsEntity_AddQuestion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "Question", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Questions", "Question");
        }
    }
}
