using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Entity;

namespace TrackerEnabledDbContext.Common.Testing
{
    [TestClass]
    public class PersistanceTests<TContext> where TContext : ITestDbContext, new()
    {
        public TContext db = new TContext();
        public DbContextTransaction transaction = null;

        [TestInitialize]
        public virtual void Initialize()
        {
            transaction = db.Database.BeginTransaction();
        }

        protected string RandomText
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }

        protected int RandomNumber
        {
            get
            {
                return new Random().Next();
            }
        }

        [TestCleanup]
        public virtual void CleanUp()
        {
            transaction.Rollback();
        }
    }
}
