namespace SampleLogMaker.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeduserIdcolumntype : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AuditLogs", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AuditLogs", "UserId", c => c.Int(nullable: false));
        }
    }
}
