namespace CPUTracking.Models.Generic
{
    public class GenericPropertyForCRUD
    {
        public virtual string? CreatedBy { get; set; } = "33e76ab2-a517-4b17-bb28-61b9200dbb12";
        public virtual DateTime CreateDate { get; set; } = DateTime.Now;
        public virtual string? LastUpdatedBy { get; set; } = "33e76ab2-a517-4b17-bb28-61b9200dbb12";
        public virtual DateTime LastUpdateDate { get; set; } = DateTime.Now;
        
    }
}
