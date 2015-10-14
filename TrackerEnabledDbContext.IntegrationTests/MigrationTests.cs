using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Models;
using TrackerEnabledDbContext.Common.Tools;

namespace TrackerEnabledDbContext.IntegrationTests
{
    [TestClass]
    public class MigrationTests: PersistanceTests<TestTrackerContext>
    {
        private LogDataMigration _migration;

        [TestInitialize]
        public void InitializeTest()
        {
            _migration = new LogDataMigration(db);

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
            var log = new AuditLog
            {
                TypeFullName = "ModelWithCustomTableAndColumnNames",
                EventType = EventType.Added,
                RecordId = RandomNumber.ToString(),
                EventDateUTC = RandomDate,
                UserName = ""
            };

            var magnitudeLogDetail = new AuditLogDetail
            {
                Log = log,
                NewValue = RandomText,
                OriginalValue = RandomText,
                PropertyName = "MagnitudeOfForce"
            };

            var directionLogDetail = new AuditLogDetail
            {
                Log = log,
                NewValue = RandomNumber.ToString(),
                OriginalValue = RandomNumber.ToString(),
                PropertyName = "Direction"
            };

            var subjectLogDetail = new AuditLogDetail
            {
                Log = log,
                NewValue = RandomText,
                OriginalValue = RandomText,
                PropertyName = "Subject"
            };

            db.AuditLog.Add(log);
            db.LogDetails.Add(magnitudeLogDetail);
            db.LogDetails.Add(directionLogDetail);
            db.LogDetails.Add(subjectLogDetail);

            db.SaveChanges();
        }
    }
}
