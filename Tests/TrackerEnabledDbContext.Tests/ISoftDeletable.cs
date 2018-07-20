namespace TrackerEnabledDbContext.Common.Tests
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }

        void Delete();
    }
}
