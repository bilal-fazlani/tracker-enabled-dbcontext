using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Tests.Extensions;
using TrackerEnabledDbContext.Common.Tests.Models;
using TrackerEnabledDbContext.Core.Common.Tests.Extensions;

namespace TrackerEnabledDbContext.Core.Tests
{
    [TestClass]
    public class UsernameConfigurationTests
    {
        protected static readonly string TestConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TEDB-Tests;Integrated Security=True;MultipleActiveResultSets=true";

        [TestMethod]
        public void Can_use_username_factory()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ConfigureUsername(() => "bilal");

                NormalModel model = new NormalModel();
                ttc.NormalModels.Add(model);
                ttc.SaveChanges();
                model.Id.AssertIsNotZero();
                model.AssertAuditForAddition(ttc, model.Id, "bilal",x => x.Id, x => x.Description);
            }
        }

        [TestMethod]
        public void Can_username_factory_override_default_username()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ConfigureUsername(() => "bilal");
                ttc.ConfigureUsername("rahul");

                NormalModel model = new NormalModel();
                ttc.NormalModels.Add(model);
                ttc.SaveChanges();
                model.Id.AssertIsNotZero();
                model.AssertAuditForAddition(ttc, model.Id,"bilal",x => x.Id,x => x.Description);
            }
        }

        [TestMethod]
        public void Can_use_default_username()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ConfigureUsername("rahul");

                NormalModel model = new NormalModel();
                ttc.NormalModels.Add(model);
                ttc.SaveChanges();
                model.Id.AssertIsNotZero();
                model.AssertAuditForAddition(ttc, model.Id,"rahul",x => x.Id,x => x.Description);
            }
        }
    }
}