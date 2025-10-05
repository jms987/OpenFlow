using OpenFlowWebServer.Repository.DbRepository;

namespace OpenFlowWebServer.Services.SecurityMethodsServices
{
    public class PasswordSecurityMethodService : AbstractSecurityMethodService
    {
        public PasswordSecurityMethodService(ISecurityRepository securityRepository) : base(securityRepository)
        {
            _fields = new List<string>() { "username", "password" };
            _securityMethodType = Enums.SecurityMethods.LoginPassword;
        }

        public override Task Validate(IDictionary<string, string> fields)
        {
            throw new NotImplementedException();
        }
    }
}
