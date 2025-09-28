namespace OpenFlowWebServer.Domain
{
    public class RunningConfiguration
    {
        public Guid ModelId { get; set; }
        public Guid DatasetId { get; set; }
        public ModelParameters Parameters { get; set; }
    }
}
