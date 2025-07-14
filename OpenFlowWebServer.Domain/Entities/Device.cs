namespace OpenFlowWebServer.Domain.Entities
{
    public class Device
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        /*public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string? DeviceDescription { get; set; }
        public string? DeviceStatus { get; set; }
        public string? DeviceLocation { get; set; }*/
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Navigation property
        public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
    }
}
