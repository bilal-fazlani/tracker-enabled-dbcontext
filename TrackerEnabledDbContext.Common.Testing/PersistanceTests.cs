using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;

namespace TrackerEnabledDbContext.Common.Testing
{
    [TestClass]
    public class PersistanceTests<TContext> where TContext : ITestDbContext, new()
    {
        public TContext db = new TContext();
        public DbContextTransaction transaction;

        protected string RandomText => Guid.NewGuid().ToString();

        protected int RandomNumber => new Random().Next(100,200);

        protected DateTime RandomDate => DateTime.Now.AddDays(-1*RandomNumber);

        protected char RandomChar
        {
            get
            {
                int num = new Random().Next(0, 26); // Zero to 25
                char let = (char)('a' + num);
                return let;
            }
        }

        [TestInitialize]
        public virtual void Initialize()
        {
            transaction = db.Database.BeginTransaction();
            GlobalTrackingConfig.Enabled = true;
            GlobalTrackingConfig.ClearFluentConfiguration();
        }

        [TestCleanup]
        public virtual void CleanUp()
        {
            transaction?.Rollback();
        }
    }
}