namespace SampleLogMaker.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial4 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Blogs", name: "Description", newName: "BlogContent");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Blogs", name: "BlogContent", newName: "Description");
        }
    }
}
