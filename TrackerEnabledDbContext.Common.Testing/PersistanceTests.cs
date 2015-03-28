using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrackerEnabledDbContext.Common.Testing
{
    [TestClass]
    public class PersistanceTests<TContext> where TContext : ITestDbContext, new()
    {
        public TContext db = new TContext();
        public DbContextTransaction transaction = null;

        protected string RandomText
        {
            get { return Guid.NewGuid().ToString(); }
        }

        protected int RandomNumber
        {
            get { return new Random().Next(); }
        }

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