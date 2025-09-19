using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SurveyBasket.API.Authentication;
using SurveyBasket.API.Contracts.Authentication;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableRateLimiting(policyName: "ipLimit")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly JwtOptions _options;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;


        public AuthController(IAuthService authService, IConfiguration configuration, IOptions<JwtOptions> options, UserManager<ApplicationUser> userManager, ILogger<AuthController> logger)
        {
            _authService = authService;
            _configuration = configuration;
            _options = options.Value;
            _userManager = userManager;
            _logger = logger;
        }
        [HttpPost("")]
        public async Task<IActionResult> GenreateToken(AuthRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Logging with email: {email} and password {passwrod}", request.email, request.password);

            var result = await _authService.GetTokenAsync(request.email, request.password, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync(RefreshTokenRequest request, CancellationToken token)
        {
            var authResult = await _authService.GenerateNewTokenAndRefreshToken(request.token, request.refreshToken, token);

            return authResult.IsSuccess ? Ok(authResult) : authResult.ToProblem();
        }

        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefresh(RefreshTokenRequest request, CancellationToken token)
        {
            var isRevoked = await _authService.Revoked(request.token, request.refreshToken, token);

            return isRevoked.IsSuccess ? Ok() : isRevoked.ToProblem();

        }




        [HttpPost("Register")]
        //[DisableRateLimiting]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        [HttpPost("ConfirmEmail")]

        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.ConfirmCode(request, cancellationToken);

            return result.IsSuccess ? Ok() : result.ToProblem();

        }


        [HttpPost("ResendConfirmEmail")]

        public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmationEmail request, CancellationToken cancellationToken)
        {
            var result = await _authService.ResendConfirmCode(request, cancellationToken);

            return result.IsSuccess ? Ok() : result.ToProblem();

        }



        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            var result = await _authService.ForgetPassword(request);

            if (!result.IsSuccess)
                return result.ToProblem();

            return Ok();
        }


        [HttpPost("Reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPassword(request);

            if (!result.IsSuccess)
                return result.ToProblem();

            return Ok();
        }


        [HttpGet("test")]

        public IActionResult Test()
        {
            Thread.Sleep(6000);
            return Ok();
        }


    }
}
