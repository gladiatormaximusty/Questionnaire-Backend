﻿namespace EIRA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Question_AddFreeTextPlaceholder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "FreeTextPlaceholder", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Questions", "FreeTextPlaceholder");
        }
    }
}
