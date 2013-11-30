namespace SampleLogMaker.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedthelogictosavevaluesinchildtable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogChilds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuditLogId = c.Guid(nullable: false),
                        Key = c.String(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuditLogs", t => t.AuditLogId, cascadeDelete: true)
                .Index(t => t.AuditLogId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuditLogChilds", "AuditLogId", "dbo.AuditLogs");
            DropIndex("dbo.AuditLogChilds", new[] { "AuditLogId" });
            DropTable("dbo.AuditLogChilds");
        }
    }
}
