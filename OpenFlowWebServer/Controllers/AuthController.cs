using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenFlowWebServer.Services;
using System.IdentityModel.Tokens.Jwt;

namespace OpenFlowWebServer.Controllers
{
    [ApiController]
    [Route("auth")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IJWTService _jwtService;
        private readonly ILoginServices _loginServices;

        public AuthController(IJWTService jwtService, ILoginServices loginServices)
        {
            _jwtService = jwtService;
            _loginServices = loginServices;
        }

        [HttpPost("login/password")]
        public async Task<IActionResult> PasswordLogin(LoginRequest request)
        {
            return await _loginServices.PasswordLogin(request.Username,request.Password);
        }
        [HttpPost("login/secret")]
        public async Task<IActionResult> SecretLogin([FromBody]SecretLoginRequest request)
        {
            return await _loginServices.SecretLogin(request.Secret);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("token/refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var token = GetToken();
            if (token == null || !_jwtService.ValidateToken(token))
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            return Ok(_jwtService.RefreshToken(token));
        }

        public class LoginRequest
        {
            public required string Username { get; set; }
            public required string Password { get; set; }
        }

        public class SecretLoginRequest
        {
            public required string Secret { get; set; }
        }

        private string? GetToken()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            string? token = null;
            if (authHeader.StartsWith("Bearer "))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }

            return token;
        }
    }
}
