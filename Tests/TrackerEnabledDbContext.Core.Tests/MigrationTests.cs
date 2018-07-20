using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Tests.Code;
using TrackerEnabledDbContext.Common.Tools;
using TrackerEnabledDbContext.Core.Common.Tests;
using TrackerEnabledDbContext.Core.Common.Tools;

namespace TrackerEnabledDbContext.Core.Tests
{
    [TestClass]
    public class MigrationTests
    {
        protected static readonly string TestConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TEDB-Tests;Integrated Security=True;MultipleActiveResultSets=true";
        protected RandomDataGenerator rdg = new RandomDataGenerator();
        private LogDataMigration _migration;

        [TestInitialize]
        public void InitializeTest()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                _migration = new LogDataMigration(ttc);                
            }

            _migration.AuditLogUpdated += (sender, args) =>
            {
                Debug.WriteLine($"\tAuditLog[{args.RecordId}] updated: {args.OldName} -> {args.NewName}");
            };

            _migration.AuditLogDetailUpdated += (sender, args) =>
            {
                Debug.WriteLine($"\t\tAuditLogDetail[{args.RecordId}] updated: {args.OldName} -> {args.NewName}");
            };
        }

        [TestMethod]
        public void CanMigrateOldLogDataToVersion3()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //arrange
            InsertFakeLegacyLog();

            bool entityNameChange = false;
            bool magnitudePropertyNameChanged = false;
            bool subjectPropertyChanged = false;
            bool directionPropertyChanged = false;

            _migration.AuditLogUpdated += (sender, args) =>
            {
                if (args.OldName == "ModelWithCustomTableAndColumnNames")
                {
                    entityNameChange = true;
                }
            };

            _migration.AuditLogDetailUpdated += (sender, args) =>
            {
                if (args.OldName == "MagnitudeOfForce")
                {
                    magnitudePropertyNameChanged = true;
                }

                if (args.OldName == "Subject")
                {
                    subjectPropertyChanged = true;
                }

                if (args.OldName == "Direction")
                {
                    directionPropertyChanged = true;
                }

            };

            //act

            _migration.MigrateLegacyLogData();

            //assert
            Assert.IsTrue(entityNameChange, "entity name not changed");
            Assert.IsTrue(magnitudePropertyNameChanged, "magnitude property name was not supposed to change");
            Assert.IsFalse(directionPropertyChanged, "direction property should not change");
            Assert.IsFalse(subjectPropertyChanged,"sunject property should not change");
        }

        [TestMethod]
        public async Task CanMigrateOldLogDataToVersion3ASyncWithProgress()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //arrange
            Debug.WriteLine("Migration started");

            IProgress<MigrationJobStatus> progress = new Progress<MigrationJobStatus>(status =>
            {
                Debug.WriteLine($"{status.Percent}% processed.... {status.EntityFullName}");
            });

            //arrange
            InsertFakeLegacyLog();

            bool entityNameChange = false;
            bool magnitudePropertyNameChanged = false;
            bool subjectPropertyChanged = false;
            bool directionPropertyChanged = false;

            _migration.AuditLogUpdated += (sender, args) =>
            {
                if (args.OldName == "ModelWithCustomTableAndColumnNames")
                {
                    entityNameChange = true;
                }
            };

            _migration.AuditLogDetailUpdated += (sender, args) =>
            {
                if (args.OldName == "MagnitudeOfForce")
                {
                    magnitudePropertyNameChanged = true;
                }

                if (args.OldName == "Subject")
                {
                    subjectPropertyChanged = true;
                }

                if (args.OldName == "Direction")
                {
                    directionPropertyChanged = true;
                }
            };

            //act
            await _migration.MigrateLegacyLogDataAsync(progress);

            //assert
            Assert.IsTrue(entityNameChange, "entity name not changed");
            Assert.IsTrue(magnitudePropertyNameChanged, "magnitude property name was not supposed to change");
            Assert.IsFalse(directionPropertyChanged, "direction property should not change");
            Assert.IsFalse(subjectPropertyChanged, "sunject property should not change");
        }

        private void InsertFakeLegacyLog()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            var log = new AuditLog
            {
                TypeFullName = "ModelWithCustomTableAndColumnNames",
                EventType = EventType.Added,
                RecordId = rdg.Get<int>().ToString(),
                EventDateUTC = rdg.Get<DateTime>(),
                UserName = ""
            };

            var magnitudeLogDetail = new AuditLogDetail
            {
                Log = log,
                NewValue = rdg.Get<string>(),
                OriginalValue = rdg.Get<string>(),
                PropertyName = "MagnitudeOfForce"
            };

            var directionLogDetail = new AuditLogDetail
            {
                Log = log,
                NewValue = rdg.Get<int>().ToString(),
                OriginalValue = rdg.Get<int>().ToString(),
                PropertyName = "Direction"
            };

            var subjectLogDetail = new AuditLogDetail
            {
                Log = log,
                NewValue = rdg.Get<string>(),
                OriginalValue = rdg.Get<string>(),
                PropertyName = "Subject"
            };

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.AuditLogs.Add(log);
                ttc.AuditLogDetails.Add(magnitudeLogDetail);
                ttc.AuditLogDetails.Add(directionLogDetail);
                ttc.AuditLogDetails.Add(subjectLogDetail);

                ttc.SaveChanges();
            }
        }
    }
}
