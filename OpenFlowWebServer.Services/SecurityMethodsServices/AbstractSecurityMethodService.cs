using OpenFlowWebServer.Enums;
using OpenFlowWebServer.Repository.DbRepository;

namespace OpenFlowWebServer.Services.SecurityMethodsServices
{

    public interface ISecurityMethodService
    {
        public string SecurityMethodType { get; }
        public int FieldsNumber { get; }
        Task Validate(IDictionary<string, string> fields);
    }

    public abstract class AbstractSecurityMethodService : ISecurityMethodService
    {
        protected readonly ISecurityRepository _securityRepository;
        protected IEnumerable<string> _fields;
        protected SecurityMethods _securityMethodType { get; set; }

        public string SecurityMethodType
        {
            get { return _securityMethodType.ToString(); }
        }
        public int FieldsNumber
        {
            get { return _fields.Count(); }
        }

        public AbstractSecurityMethodService(ISecurityRepository securityRepository)
        {
            _securityRepository = securityRepository;
        }

        public abstract Task Validate(IDictionary<string, string> fields);
    }
}
