namespace OpenFlowWebServer.Data.Domain
{
    public class File
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Container { get; set; }
        public Guid BlobGuid { get; set; }
        public string Extension { get; set; }
        public string Name { get; set; }
    }
}
