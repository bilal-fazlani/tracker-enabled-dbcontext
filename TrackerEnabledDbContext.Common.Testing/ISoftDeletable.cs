namespace TrackerEnabledDbContext.Common.Testing
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }

        void Delete();
    }
}
