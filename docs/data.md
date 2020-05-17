Tracked data is stored in 2 tables. And note that each one has a corresponding DbSet<> property in TrackerContext so you can do all the CRUD you want.

## AuditLog

Stores entity level tracking information, like when an entity was changed, who changed it, what was the change ( insert /update/ delete ), etc.

![auditlog](/imgs/auditlog.png)

## AuditLogDetails

Stores property level tracking information like, if an entity property was modified what was its old value and what is its new value.

![auditlogdetail](/imgs/auditlogdetails.png)

## Querying data

You have the freedom to query this tracking data manually from these tables.

Or you can query the tracking data using built-in API as follows-

```c#
using (Context ctx = new Context())
{
    IQueryable<AuditLog> allCarLogs = ctx.GetLogs<Car>();

    Car myCar = ctx.Cars.Single(x => x.Number == "JH-876G");

    IQueryable<AuditLog> myCarLogs = ctx.GetLogs<Car>(myCar.Id);
}
```