namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_QuestionnairesFinishedEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionnairesFinished",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionnairesId = c.Int(nullable: false),
                        BUId = c.Int(nullable: false),
                        BUName = c.String(),
                        EntityQuestionsJson = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QuestionnairesFinished");
        }
    }
}
