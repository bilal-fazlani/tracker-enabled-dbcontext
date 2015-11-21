using System;
using System.Data.Entity;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Testing.Code;

namespace TrackerEnabledDbContext.Common.Testing
{
    [TestClass]
    public class PersistanceTests<TContext> where TContext : ITestDbContext, new()
    {
        private const string TestConnectionString = "DefaultTestConnection";
        private readonly RandomDataGenerator _randomDataGenerator = new RandomDataGenerator();

        public TContext db = new TContext();

        public DbContextTransaction transaction;

        protected bool RollBack = true;

        protected string RandomText => _randomDataGenerator.Get<string>();

        protected int RandomNumber => _randomDataGenerator.Get<int>();

        protected DateTime RandomDate => _randomDataGenerator.Get<DateTime>();

        protected char RandomChar => _randomDataGenerator.Get<char>();

        protected ObjectFactory<T, TContext> GetObjectFactory<T>() where T:class
        {
            return new ObjectFactory<T, TContext>();
        }

        [TestInitialize]
        public virtual void Initialize()
        {
            transaction = db.Database.BeginTransaction();
            GlobalTrackingConfig.Enabled = true;
            GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion = false;
            GlobalTrackingConfig.DisconnectedContext = false;
            GlobalTrackingConfig.ClearFluentConfiguration();
        }

        [TestCleanup]
        public virtual void CleanUp()
        {
            if (RollBack)
            {
                transaction?.Rollback();
            }
            else
            {
                transaction?.Commit();
            }
        }
    }
}