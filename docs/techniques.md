You can save data using entity framework in 2 ways

## The Normal way

```c#
public ActionResult Save(BlogViewModel blogViewModel)
{
    Blog blog = _dbContext.Blogs.Find(blogViewModel.Id);

    blog.Description = blogViewModel.Description;

    blog.Title = blogViewModel.Title;

    _dbContext.SaveChanges();

    return View();
}
```

## Manually attaching entities

```c#
public ActionResult Save(Blog blog)
{
    _dbContext.Blogs.Attach(blog);
    _dbContext.Entry(blog).State = EntityState.Modified;

    _dbContext.SaveChanges();

    return View();
}
```

In this pattern, TEDB wont work by default. You will have to set 

```c#
GlobalTrackingConfig.DisconnectedContext = true;
```

on application startup.

