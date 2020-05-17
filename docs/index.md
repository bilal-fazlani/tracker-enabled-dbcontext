tracker-enabled-dbcontext (TEDB) is a .net library based on entity framework. It is created for tracking the changes in database. This library will records new record additions, record changes & record deletions. When recording record modifications, it will audit previous and new values of the fields. It will also audit the time of change and user who changed/added/deleted the record.

## Installation

Install the desired nuget package using nuget package manager or package manager console.

You can install using console, like this- 

```
Install-Package TrackerEnabledDbContext
```

OR
    
```
Install-Package TrackerEnabledDbContext.Identity
```

If you want to try upcoming beta features, use the following command

```
Install-Package TrackerEnabledDbContext -Pre
```
