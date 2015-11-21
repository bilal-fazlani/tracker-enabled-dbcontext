using System;

namespace TrackerEnabledDbContext.Common.Testing.Code
{
    public class ObjectFactory<TEntity, TContext>
        where TEntity : class 
        where TContext : ITestDbContext, new()
    {
        readonly ObjectFiller<TEntity> _filler= new ObjectFiller<TEntity>();

        public ObjectFactory()
        {
            _filler.IgnorePropertiesWhen(propName => 
                propName.EndsWith("Id") || 
                propName == "IsDeleted");
        }

        public TEntity Create(bool fill = true, bool save = false, ITestDbContext testDbContext = null)
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