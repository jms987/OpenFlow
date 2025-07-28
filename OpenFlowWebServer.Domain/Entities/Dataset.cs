namespace OpenFlowWebServer.Domain.Entities
{
    public class Dataset
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public Guid ProjectId { get; set; }
        public string? Description { get; set; }
        public Guid? ConfigFileId { get; set; }
        public Guid? DatasetFileId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        
        public virtual File ConfigFile { get; set; }
        public virtual File DatasetFile { get; set; }

        /*public string? DatasetType { get; set; }
        public string? DatasetLocation { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;*/
        // Navigation property
    }
}
