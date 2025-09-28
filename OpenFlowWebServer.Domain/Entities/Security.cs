using OpenFlowWebServer.Enums;

namespace OpenFlowWebServer.Domain.Entities
{
    public class Security
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public SecurityMethods Method { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
        public virtual Project Project { get; set; }
    }
}
