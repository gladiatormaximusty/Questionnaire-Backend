namespace EIRA.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Add_QuestionnairesAsignEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionnairesAsign",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SingleAnswer = c.Boolean(nullable: false),
                        SelectedAnswer = c.String(),
                        FreeText = c.String(),
                        AnswerTime = c.String(),
                        AnswerUserId = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                        BU_Id = c.Int(),
                        Entity_Id = c.Int(),
                        Question_Id = c.Int(),
                        Questionnaire_Id = c.Int(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_QuestionnairesAsign_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BUs", t => t.BU_Id)
                .ForeignKey("dbo.Entities", t => t.Entity_Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .ForeignKey("dbo.Questionnaires", t => t.Questionnaire_Id)
                .Index(t => t.IsDeleted)
                .Index(t => t.BU_Id)
                .Index(t => t.Entity_Id)
                .Index(t => t.Question_Id)
                .Index(t => t.Questionnaire_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionnairesAsign", "Questionnaire_Id", "dbo.Questionnaires");
            DropForeignKey("dbo.QuestionnairesAsign", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.QuestionnairesAsign", "Entity_Id", "dbo.Entities");
            DropForeignKey("dbo.QuestionnairesAsign", "BU_Id", "dbo.BUs");
            DropIndex("dbo.QuestionnairesAsign", new[] { "Questionnaire_Id" });
            DropIndex("dbo.QuestionnairesAsign", new[] { "Question_Id" });
            DropIndex("dbo.QuestionnairesAsign", new[] { "Entity_Id" });
            DropIndex("dbo.QuestionnairesAsign", new[] { "BU_Id" });
            DropIndex("dbo.QuestionnairesAsign", new[] { "IsDeleted" });
            DropTable("dbo.QuestionnairesAsign",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_QuestionnairesAsign_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
