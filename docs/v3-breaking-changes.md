# Version 3.0 Breaking changes

#### `TypeFullName` instead of `TableName` & `PropertyName` instead of `Columnname`
This makes it more unique to store and avoid a lot of confusion. It will also help in developing some more upcoming features.

#### Annotations now do not take any parameters
It was simply un-necessary and confusing.

#### Primary keys type of audit tables changed
The type of primary key columns of audit tables was int. It has been changed to long considering that number of logs can increase very rapidly.

### How to upgrade from version 2.7 to 3.0

#### Step 1: Run this migration code against your database to change the schema.
```c#
public override void Up()
{
    //drop dependencies

    DropIndex("AuditLogDetails", new[] { "AuditLogId" });
    DropForeignKey("dbo.AuditLogDetails", "AuditLogId", "dbo.AuditLogs");
    DropPrimaryKey("dbo.AuditLogDetails");
    DropPrimaryKey("dbo.AuditLogs");

    //main changes

    RenameColumn("dbo.AuditLogs", "TableName", "TypeFullName");
    RenameColumn("dbo.AuditLogDetails", "ColumnName", "PropertyName");
    AlterColumn("dbo.AuditLogDetails", "Id", builder => builder.Long(false, true, null));
    AlterColumn("dbo.AuditLogs", "AuditLogId", builder => builder.Long(false, true, null));
    AlterColumn("dbo.AuditLogDetails", "AuditLogId", builder => builder.Long(false));

    //add dependencies again

    AddPrimaryKey("dbo.AuditLogDetails", "Id");
    AddPrimaryKey("dbo.AuditLogs", "AuditLogId");
    AddForeignKey("dbo.AuditLogDetails", "AuditLogId", "dbo.AuditLogs");
    CreateIndex("dbo.AuditLogDetails", "AuditLogId");
}

public override void Down()
{
}
```
To know more about running migrations visit this [MSDN article](https://msdn.microsoft.com/en-in/data/jj591621.aspx#generating)

#### Step 2: Run this code (one time) to migrate your legacy log data to new version

```c#
var migration = new LogDataMigration(db);

using (var transaction = db.Database.BeginTransaction())
{
    try
    {
        migration.MigrateLegacyLogData();
        transaction.Commit();
    }
    catch (Exception)
    {
        transaction.Rollback();
    }
}
```            