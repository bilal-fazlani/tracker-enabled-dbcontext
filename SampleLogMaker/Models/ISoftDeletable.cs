namespace SampleLogMaker.Models
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}