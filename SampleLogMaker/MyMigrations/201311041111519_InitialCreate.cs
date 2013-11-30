namespace SampleLogMaker.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        AuditLogID = c.Guid(nullable: false),
                        UserId = c.Int(nullable: false),
                        EventDateUTC = c.DateTime(nullable: false),
                        EventType = c.String(nullable: false, maxLength: 1),
                        TableName = c.String(nullable: false, maxLength: 256),
                        RecordID = c.String(nullable: false, maxLength: 256),
                        ColumnName = c.String(nullable: false, maxLength: 256),
                        OriginalValue = c.String(),
                        NewValue = c.String(),
                    })
                .PrimaryKey(t => t.AuditLogID);
            
            CreateTable(
                "dbo.Blogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Blogs");
            DropTable("dbo.AuditLogs");
        }
    }
}
