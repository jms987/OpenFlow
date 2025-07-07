namespace OpenFlowWebServer.Data.Domain
{
    public class Log
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string,string> LogData { get; set; } = new Dictionary<string, string>();
        public virtual Device Device
        {
            get; set;
        }
    }
}
