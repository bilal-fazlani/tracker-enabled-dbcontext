With version 3.5.20-beta onwards, you can configure username logic just once and wont have to specify it every time you call `SaveChanges()` or `SaveChangesAsync()`. Although you will still have the option to specify/override username with `SaveChanges(string)` and `SaveChangesAsync(string)` methods. 

### Configuring default username logic

```c#
    public class SampleController : Controller
    {
        private ApplicationDbContext _dbContext;

        public SampleController()
        {
            _dbContext = new ApplicationDbContext();
            _dbContext.ConfigureUsername(()=>User.Identity.Name);
        }

        // GET: Sample
        public ActionResult Save()
        {
            _dbContext.SaveChanges();
            return View();
        }
    }
```

The above code will always insert currently logged in user's username and so we don't need to specify the username in `_dbContext.SaveChanges();` call.

### Configuring default username or anonymous username

You can also specify what to insert when there is no username given. The code is very similar to above code except for you pass in a hardcoded string instead of a delegate. 

```c#
    public class SampleController : Controller
    {
        private ApplicationDbContext _dbContext;
        const string AnonymousUsername = "Anonymous";

        public SampleController()
        {
            _dbContext = new ApplicationDbContext();
            _dbContext.ConfigureUsername(AnonymousUsername);
        }

        // GET: Sample
        public ActionResult Save()
        {
            _dbContext.SaveChanges();
            return View();
        }
    }
```

### Overriding username

```c#
_dbContext.SaveChanges("bilal");
```