using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using SurveyBasket.API.Authentication;
using SurveyBasket.API.Contracts.Authentication;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly JwtOptions _options;
        
        public AuthController(IAuthService authService,IConfiguration configuration,IOptions<JwtOptions> options)
        {
            _authService = authService;
            _configuration = configuration;
            _options = options.Value;
        }
        [HttpPost("")]
        public async Task<IActionResult> GenreateToken(AuthRequest request,CancellationToken cancellationToken)
        {
            var result = await _authService.GetTokenAsync(request.email, request.password,cancellationToken);
            if (result is null)
                return BadRequest();
            return Ok(result);
        }

        [HttpGet("config")]
        public IActionResult Test()
        {
            var config = new
            {
                Key=_options.Key,
                Issuer=_options.Issuer,
                Audience=_options.Audience,
                ExpiryMinutes=_options.ExpiryMinutes
            };

            return Ok(config);
        }

    }
}
