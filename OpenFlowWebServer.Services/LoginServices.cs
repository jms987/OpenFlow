using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Enums;
using OpenFlowWebServer.Repository.DbRepository;

namespace OpenFlowWebServer.Services
{
    public interface ILoginServices
    {
        public Task<IActionResult> PasswordLogin(string username, string password);
        public Task<IActionResult> SecretLogin(string secret);

    }
    public class LoginServices: ILoginServices
    {
        private IProjectRepository _projectRepository { get; set; }
        private IDeviceRepository _deviceRepository { get; set; }
        private IJWTService _jwtService { get; set; }

        public LoginServices(IProjectRepository projectRepository, IDeviceRepository deviceRepository, IJWTService jwtService)
        {
            _projectRepository = projectRepository;
            _deviceRepository = deviceRepository;
            _jwtService = jwtService;
        }

        public async Task<IActionResult> PasswordLogin(string username, string password)
        {
            var project = await GetProject();
            if (ReferenceEquals(project, null))
            {
                return new NoContentResult();
            }
            if (project.Securities.Any(s => s.Method == SecurityMethods.LoginPassword))
            {
                var security = project.Securities.First(s => s.Method == SecurityMethods.LoginPassword);
                if (security.Parameters["username"] == username && security.Parameters["password"] == password)
                {
                    return await RegisterDevice();
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            else
            {
                return new ContentResult { StatusCode = StatusCodes.Status405MethodNotAllowed, Content = "Currently deployed project do not use Password login" };
            }
        }
        
        public async Task<IActionResult> SecretLogin(string secret)
        {
            var project = await GetProject();
            if (ReferenceEquals(project, null))
            {
                return new NoContentResult();
            }
            if (project.Securities.Any(s => s.Method == SecurityMethods.Secret))
            {
                var security = project.Securities.First(s => s.Method == SecurityMethods.Secret);
                if (security.Parameters["secret"] == secret)
                {
                    return await RegisterDevice();
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            else
            {
                return new ContentResult { StatusCode = StatusCodes.Status405MethodNotAllowed, Content = "Currently deployed project do not use Secret login" };
            }
        }

        private async Task<Project> GetProject()
        {
            var project = await _projectRepository.GetDeployedProject();
            if (project == null)
                throw new NullReferenceException("No deployed project found.");
            return project;
        }

        private async Task<IActionResult> RegisterDevice()
        {
            var device = new Device()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
            };
            await _deviceRepository.AddAsync(device);
            await _deviceRepository.SaveChangesAsync();
            var jwt = _jwtService.GenerateToken(new Claim[]
            {
                new Claim("deviceId", device.Id.ToString())
            });
            return new OkObjectResult(jwt);
        }

    }
}
