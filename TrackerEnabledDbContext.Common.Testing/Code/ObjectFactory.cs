using System;

namespace TrackerEnabledDbContext.Common.Testing.Code
{
    public class ObjectFactory<TContext>
        
        where TContext : ITestDbContext, new()
    {
        readonly ObjectFiller _filler = new ObjectFiller();

        public ObjectFactory()
        {
            _filler.IgnorePropertiesWhen(propName => 
                propName.EndsWith("Id") || 
                propName == "IsDeleted");
        }

        public TEntity Create<TEntity>
            (bool fill = true, bool save = false, ITestDbContext testDbContext = null)
            where TEntity : class
        {
            var instance = Activator.CreateInstance<TEntity>();

            if (fill)
            {
                _filler.Fill(instance);
            }

            if (save)
            {
                if (testDbContext == null)
                {
                    using (var db = new TContext())
                    {
                        db.Set<TEntity>().Add(instance);
                        db.SaveChanges();
                    }
                }
                else
                {
                    testDbContext.Set<TEntity>().Add(instance);
                    testDbContext.SaveChanges();
                }
            }

            return instance;
        }
    }
}