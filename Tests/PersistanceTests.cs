using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;

namespace Tests
{
    [TestClass]
    public class PersistanceTests
    {
        public TestTrackerContext db = new TestTrackerContext();
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
