Number of logs can become very large and that's when you might want to save all your logs to some other location. It could be a network location or disk file or a secondary remote database just meant for logs. Version 3.5 and greater has the provision to let you decide where you want to save the logs.

`OnAuditLogGenerated` event is raised after log is generated but before saving the log to database. It exposes the the generated log in the event argument. Which you can then use to save somewhere else.

!!! Caution
     If you make any changes to this log object, those changes will be saved

Using the event arg you can also instruct it to skip saving of generated log to (local) database to avoid redundancy, which is what most people would want to do if they have already saved the generated to some other location. 

Example Code:

```c#
db.OnAuditLogGenerated += (sender, args) =>
{
    var auditLog = args.Log;

    DumpLogToAnotherDatabase(auditLog);

    //skips saving to local database
    args.SkipSaving = true;
};
```

Note that this event will be raised only for models which are set for tracking (by either fluent or attribute configuration). And will only contain model properties which are enabled for tracking (Obviously).


## AuditLogGeneratedEventArgs

**`AuditLog Log`**

This is the log that is generated. You can read log details from this object but if you make make changes to it, those changes will be persisted to application database and Serilog sinks.

**`object Entity`**

This is the entity for which the event has been raised. If you make changes to this entity, changes will be saved to database and **these changes will not be logged.**

**`bool SkipSavingLog`**

False by default. When set to true, it won't save current log to database. 

**`dynamic Metadata`**

This is a dynamic object and contains [Metadata](/metadata).