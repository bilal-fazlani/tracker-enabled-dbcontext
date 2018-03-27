namespace TrackerEnabledDbContext.EF6.Common.Testing
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }

        void Delete();
    }
}
