namespace EIRA.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Add_QuestionsAsign : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionsAsign",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Questionnaire_Id = c.Int(nullable: false),
                        Question_Id = c.Int(nullable: false),
                        SingleAnswer = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_QuestionsAsign_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id, cascadeDelete: true)
                .ForeignKey("dbo.Questionnaires", t => t.Questionnaire_Id, cascadeDelete: true)
                .Index(t => t.Questionnaire_Id)
                .Index(t => t.Question_Id)
                .Index(t => t.IsDeleted);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionsAsign", "Questionnaire_Id", "dbo.Questionnaires");
            DropForeignKey("dbo.QuestionsAsign", "Question_Id", "dbo.Questions");
            DropIndex("dbo.QuestionsAsign", new[] { "IsDeleted" });
            DropIndex("dbo.QuestionsAsign", new[] { "Question_Id" });
            DropIndex("dbo.QuestionsAsign", new[] { "Questionnaire_Id" });
            DropTable("dbo.QuestionsAsign",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_QuestionsAsign_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
