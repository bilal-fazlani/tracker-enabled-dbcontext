namespace SampleLogMaker.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedforkeyforblog : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Comments", name: "ParentBlog_Id", newName: "ParentBlogId");
            RenameIndex(table: "dbo.Comments", name: "IX_ParentBlog_Id", newName: "IX_ParentBlogId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Comments", name: "IX_ParentBlogId", newName: "IX_ParentBlog_Id");
            RenameColumn(table: "dbo.Comments", name: "ParentBlogId", newName: "ParentBlog_Id");
        }
    }
}
