namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_QuestionnairesAsignEntity_Type : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QuestionnairesAsign", "AnswerTime", c => c.DateTime());
            AlterColumn("dbo.QuestionnairesAsign", "AnswerUserId", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QuestionnairesAsign", "AnswerUserId", c => c.String());
            AlterColumn("dbo.QuestionnairesAsign", "AnswerTime", c => c.String());
        }
    }
}
