namespace EIRA.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Add_QuestionsAnswerEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionsAnswer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AnswerContent = c.String(),
                        RecommendedScore = c.String(),
                        Status = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                        Question_Id = c.Int(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_QuestionsAnswer_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.IsDeleted)
                .Index(t => t.Question_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionsAnswer", "Question_Id", "dbo.Questions");
            DropIndex("dbo.QuestionsAnswer", new[] { "Question_Id" });
            DropIndex("dbo.QuestionsAnswer", new[] { "IsDeleted" });
            DropTable("dbo.QuestionsAnswer",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_QuestionsAnswer_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
