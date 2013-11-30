namespace SampleLogMaker.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedaufitlogdatetimecolumntodatetimeoffset : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AuditLogs", "EventDateUTC", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AuditLogs", "EventDateUTC", c => c.DateTime(nullable: false));
        }
    }
}
