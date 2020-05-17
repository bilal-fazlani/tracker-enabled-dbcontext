Now you save can save metadata with your audit logs. For example you might want to save the ip address of the request that caused the change. Note that metadata can be anything and not just an ip address. It is completely up to you what and how much you want to audit.

### Example
```c#
public class CommentsController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    public CommentsController()
    {
        db.ConfigureMetadata(metadata =>
        {
            metadata.IpAddress = Request.UserHostAddress;
            metadata.RequestDevice = "AndroidPhone";
            metadata.Country = Request.Cookies["country"];
        });
    }
}
```
### Notes
 - `ConfigureMetadata` method accepts an `Action<dynamic>` so you can do Metadata.`ANYTHING` = `ANYTHING`.

 - Metadata is saved in a new table called `dbo.LogMetadata`. You can override this using fluent api of entity framework.

 - `AuditLog` now contains a new property - `ICollection<LogMetadata> Metadata`

 - and here's how `LogMetadata` looks like:

```c#
public class LogMetadata
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public virtual long AuditLogId { get; set; }

    [ForeignKey("AuditLogId")]
    public virtual AuditLog AuditLog { get; set; }
        
    public string Key { get; set; }

    public string Value { get; set; }
}
```

 - And here is how it looks like in the database:


```sql
[Id] [bigint] IDENTITY(1,1) NOT NULL,
[AuditLogId] [bigint] NOT NULL,
[Key] [nvarchar](max) NULL,
[Value] [nvarchar](max) NULL, /*NOTE VALUE IS STORED AS NVARCHAR*/
```

 - The `OnAuditLogGenerated` event gives you direct access of metadata in case you need it. For example.

```c#
db.OnAuditLogGenerated += (sender, args) =>
{
    string ipAddress = args.Metadata.IpAddress;
    string device = args.Metadata.RequestDevice;
    string country = args.Metadata.Country;
};
```

!!! Note
    Here too, args.Metadata is dynamic so you can try to access anything - args.Metadata.`XXXXX`. But if `XXXXX` was never assigned, it will throw `RutimeBinderException`.