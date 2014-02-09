namespace SampleLogMaker.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial3 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "scb.TheBlogTable", newName: "Blogs");
            MoveTable(name: "scb.Blogs", newSchema: "dbo");
        }
        
        public override void Down()
        {
            MoveTable(name: "dbo.Blogs", newSchema: "scb");
            RenameTable(name: "scb.Blogs", newName: "TheBlogTable");
        }
    }
}
