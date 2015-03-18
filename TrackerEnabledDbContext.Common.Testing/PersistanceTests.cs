using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext.Common.Testing
{
    [TestClass]
    public class PersistanceTests<TContext> where TContext : ITrackerContext, new()
    {
        public TContext db = new TContext();
        public DbContextTransaction transaction = null;

        [TestInitialize]
        public virtual void Initialize()
        {
            transaction = db.Database.BeginTransaction();
        }

        [TestCleanup]
        public virtual void CleanUp()
        {
            transaction.Rollback();
        }

    }
}
