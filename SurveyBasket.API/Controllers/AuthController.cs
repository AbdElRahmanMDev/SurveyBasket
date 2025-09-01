using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using SurveyBasket.API.Abstraction;
using SurveyBasket.API.Authentication;
using SurveyBasket.API.Contracts.Authentication;
using SurveyBasket.API.Errors;
using System.Reflection;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly JwtOptions _options;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;   

        public AuthController(IAuthService authService,IConfiguration configuration,IOptions<JwtOptions> options,UserManager<ApplicationUser> userManager, ILogger<AuthController> logger)
        {
            _authService = authService;
            _configuration = configuration;
            _options = options.Value;
            _userManager = userManager;
            _logger = logger;
        }
        [HttpPost("")]
        public async Task<IActionResult> GenreateToken(AuthRequest request,CancellationToken cancellationToken)
        {
            _logger.LogInformation("Logging with email: {email} and password {passwrod}",request.email,request.password);

            var result = await _authService.GetTokenAsync(request.email, request.password,cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync(RefreshTokenRequest request,CancellationToken token)
        {
          var authResult= await _authService.GenerateNewTokenAndRefreshToken(request.token,request.refreshToken,token);

            return authResult.IsSuccess ? Ok(authResult) : authResult.ToProblem();
        }

        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefresh(RefreshTokenRequest request, CancellationToken token)
        {
            var isRevoked = await _authService.Revoked(request.token, request.refreshToken, token);

            return isRevoked.IsSuccess ? Ok() : isRevoked.ToProblem();
        
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(AuthRequest model)
        {
            var user = new ApplicationUser()
            {
                Email = model.email,
                UserName= model.email

            };

            var result = await _userManager.CreateAsync(user, model.password);

            if(result.Succeeded)
            {
                return Ok(new { message = "User registered successfuly" });

            }
            return BadRequest(result.Errors);
        }
    }
}
