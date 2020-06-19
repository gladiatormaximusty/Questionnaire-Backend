namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_SupportingDocumentEntiy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SupportingDocument",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PathUrl = c.String(),
                        Type = c.String(),
                        ContentType = c.String(),
                        QuestionnairesAsign_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SupportingDocument");
        }
    }
}
