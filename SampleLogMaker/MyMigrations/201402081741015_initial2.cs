namespace SampleLogMaker.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Blogs", newName: "TheBlogTable");
            MoveTable(name: "dbo.TheBlogTable", newSchema: "scb");
        }
        
        public override void Down()
        {
            MoveTable(name: "scb.TheBlogTable", newSchema: "dbo");
            RenameTable(name: "dbo.TheBlogTable", newName: "Blogs");
        }
    }
}
