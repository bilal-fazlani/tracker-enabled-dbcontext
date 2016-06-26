using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackingCore.Events;
using TrackingCore.Models;

namespace TrackingCore.Interfaces
{
    public interface ITrackerContext : IDbContext
    {
        event EventHandler<AuditLogGeneratedEventArgs> OnAuditLogGenerated;

        void ConfigureUsername(Func<string> usernameFactory);

        void ConfigureUsername(string defaultUsername);

        void ConfigureMetadata(Action<dynamic> metadataConfiguration);

        int SaveChanges(object userName);

        Task<int> SaveChangesAsync(object userName, CancellationToken cancellationToken);

        Task<int> SaveChangesAsync(int userId);

        Task<int> SaveChangesAsync(string userName);
    }
}