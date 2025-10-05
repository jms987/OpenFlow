using OpenFlowWebServer.Repository.DbRepository;

namespace OpenFlowWebServer.Services.SecurityMethodsServices
{
    public class SecretSecurityMethodService : AbstractSecurityMethodService
    {
        public SecretSecurityMethodService(ISecurityRepository securityRepository) : base(securityRepository)
        {
            _securityMethodType = Enums.SecurityMethods.Secret;
            _fields = new List<string>(){"secret"};
        }

        public override Task Validate(IDictionary<string, string> fields)
        {
            throw new NotImplementedException();
        }
    }
}
