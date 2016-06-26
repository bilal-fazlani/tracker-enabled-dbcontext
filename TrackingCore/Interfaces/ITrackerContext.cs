using System;
using System.Threading;
using System.Threading.Tasks;
using TrackingCore.Events;

namespace TrackingCore.Interfaces
{
    public interface ITrackerContext : IDbContext
    {
        event EventHandler<DatabaseChangeEventArgs> OnDatabaseChange;

        void ConfigureUsername(Func<string> usernameFactory);

        void ConfigureUsername(string defaultUsername);

        void ConfigureMetadata(Action<dynamic> metadataConfiguration);

        int SaveChanges(object userName);

        Task<int> SaveChangesAsync(object userName, CancellationToken cancellationToken);

        Task<int> SaveChangesAsync(int userId);

        Task<int> SaveChangesAsync(string userName);
    }
}