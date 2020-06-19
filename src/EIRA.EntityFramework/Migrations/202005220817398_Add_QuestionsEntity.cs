namespace EIRA.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Add_QuestionsEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionCode = c.String(),
                        HighestRating = c.String(),
                        Status = c.String(),
                        ScoringMethod = c.String(),
                        IsAnswerMandatory = c.Boolean(nullable: false),
                        HasFreeText = c.Boolean(nullable: false),
                        IsFreeTextMandatory = c.Boolean(nullable: false),
                        IsFreeTextNumeric = c.Boolean(nullable: false),
                        HasSupportingDocument = c.Boolean(nullable: false),
                        IsSupportingDocumentMandatory = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                        QuestionType_Id = c.Int(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Questions_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuestionTypes", t => t.QuestionType_Id)
                .Index(t => t.IsDeleted)
                .Index(t => t.QuestionType_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Questions", "QuestionType_Id", "dbo.QuestionTypes");
            DropIndex("dbo.Questions", new[] { "QuestionType_Id" });
            DropIndex("dbo.Questions", new[] { "IsDeleted" });
            DropTable("dbo.Questions",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Questions_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
