# Types of TEDB

There are two TEDB libraries

1. TEDB ( [TrackerEnabledDbcontext](https://www.nuget.org/packages/TrackerEnabledDbContext) )
2. TEDB Identity ( [TrackerEnabledDbcontext.Identity](https://www.nuget.org/packages/TrackerEnabledDbContext.identity) )

## TrackerEnabledDbcontext

This is the main library. It can be used with any type of .net project. For example,
 
- Console application
- Winforms application
- Web application ( MVC/ Webforms )
- Windows store application
- Class library

Basic use:

```c#
public class ApplicationDbContext : TrackerContext
{
    public ApplicationDbContext()
        : base("DefaultConnection")
    {
    }

    public DbSet<Blog> Blogs { get; set; }
}

[TrackChanges]
public class Blog
{
    public int Id { get; set; }

    public string Title { get; set; }

    [SkipTracking]
    public string Description { get; set; }
}

```

## TrackerEnabledDbcontext.Identity

This library was later added to support Asp.Net Identity specifically.

You should install any one of them in one project depending upon the type of project. You will generally not need both of them installed in project, but you can do it if you want.

Basic use:

```c#
public class ApplicationDbContext : TrackerIdentityContext<ApplicationUser>
{
    public ApplicationDbContext()
        : base("DefaultConnection")
    {
    }

    public DbSet<Blog> Blogs { get; set; }
}
```