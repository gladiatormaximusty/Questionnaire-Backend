namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_QuestionnairesSubmissionDeadlineType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Questionnaires", "SubmissionDeadline", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Questionnaires", "SubmissionDeadline", c => c.String());
        }
    }
}
