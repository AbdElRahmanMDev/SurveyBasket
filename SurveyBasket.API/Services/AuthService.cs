using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.API.Abstraction;
using SurveyBasket.API.Abstraction.Consts;
using SurveyBasket.API.Contracts.Authentication;
using SurveyBasket.API.Contracts.Users;
using SurveyBasket.API.Entities;
using SurveyBasket.API.Errors;
using SurveyBasket.API.Helpers;
using SurveyBasket.API.Persistence;
using SurveyBasket.Authentication;
using System.ComponentModel.Design;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SurveyBasket.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly int _refreshTokenExpire = 14;
        private readonly IHttpContextAccessor _httpContextAccessor ;
        private readonly ApplicationDbContext _context;
        public AuthService(UserManager<ApplicationUser> userManager,ApplicationDbContext context,IJwtProvider jwtProvider, ILogger<AuthService> logger,IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<TResult<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken token = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials); ;

            var IsvalidaPassword =await _userManager.CheckPasswordAsync(user, password);

            if (!IsvalidaPassword)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            if(!user.EmailConfirmed)
                return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed); 


            var authResponse=await GenerateAuthResponseAsync(user);

            return Result.Succes(authResponse);
        }


        public async Task<TResult<AuthResponse>> GenerateNewTokenAndRefreshToken(string _token, string refreshToken, CancellationToken token = default)
        {

            var userId = _jwtProvider.ValidateJwt(_token);

            if (userId is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

           var user=await _userManager.FindByIdAsync(userId);
            
            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);


            var userRefreshToken =user.RefreshTokens.SingleOrDefault(x=>x.token==_token && x.IsActive);

            if (userRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;



            var (newToken, expire) =await generateToken(user);

            var newrefreshToken = GenerateRefreshToken();
            var ExpireDate = DateTime.UtcNow.AddDays(_refreshTokenExpire);
            user.RefreshTokens.Add(new RefreshToken()
            {
                token = newrefreshToken,
                ExpireOn = ExpireDate
            });

            var response = new AuthResponse(Guid.NewGuid().ToString(), email: "Test@gmail.com", FirstName: "abdo", LastName: "abdoo",
                Token: newToken,
                ExpireIn: expire,
                refreshToken: newrefreshToken,
                ExpireDate: ExpireDate);

            return Result.Succes(response);   

        }
        
        public async Task<Result> Revoked(string _token, string refreshToken, CancellationToken token = default)
        {
          var userId=  _jwtProvider.ValidateJwt(_token);
            if(userId is null) return Result.Failure(UserErrors.InvalidJwtToken);

            var user=await _userManager.FindByIdAsync(userId);
            if (user is null) return Result.Failure(UserErrors.InvalidJwtToken);

            var _refreshToken = user.RefreshTokens.FirstOrDefault(x => x.token == refreshToken && x.IsActive);
            if (_refreshToken is null) return Result.Failure(UserErrors.InvalidRefreshToken);

            _refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return Result.Succes();
                

        }

        public async Task<Result> RegisterAsync(SurveyBasket.API.Authentication.RegisterRequest request,CancellationToken cancellationToken = default)
        {
            var emaiIsExists = await _userManager.FindByEmailAsync(request.Email);
            if(emaiIsExists is not null)
                return Result.Failure<AuthResponse>(UserErrors.DuplicateEmail);

            var user = new ApplicationUser()
            {
                Email=request.Email,
                UserName=request.Email,
                LastName = request.LastName,
                FirstName = request.FirstName,

            };

            var createUser = await _userManager.CreateAsync(user, request.Password);

            if (createUser.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code=WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));


                _logger.LogInformation("Confirmation Code {code}", code);


                await SendConfirmationEmail(user, code);

                return Result.Succes();
            }
            var errors = createUser.Errors.First();
                
            return Result.Failure<AuthResponse>(new Error(errors.Code, errors.Description,StatusCodes.Status400BadRequest));


        }



        public async Task<Result> ForgetPassword(ForgetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Result.Succes();

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            _logger.LogInformation("Confirmation Code {code}", code);

            await SendResetPasswordEmail(user, code);
            return Result.Succes();

        }

        public async Task<Result> ResetPassword(SurveyBasket.API.Authentication.ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user is null || !user.EmailConfirmed)
                return Result.Failure(UserErrors.InvalidCode);

            IdentityResult result;

            try
            {
               var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                result = await _userManager.ResetPasswordAsync(user, request.Code, request.NewPassword);

            }
            catch (FormatException)
            {
                result= IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            if (result.Succeeded)
                return Result.Succes();

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));


        }

        public async Task<Result> ConfirmCode(ConfirmEmailRequest confirmEmailRequest, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(confirmEmailRequest.UserId);
            if(user is null)
                return Result.Failure(UserErrors.InvalidCode);

            if(user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);
            var code = confirmEmailRequest.Code;

            try
            {
                code=Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch(FormatException)
            {
                return Result.Failure(UserErrors.InvalidCode);
            }

            var result=_userManager.ConfirmEmailAsync(user, code);

            if (result.Result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, DefaultRoles.MemberRoleName);

                return Result.Succes();

            }
            var error = result.Result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

        }
        public async Task<Result> ResendConfirmCode(ResendConfirmationEmail resendConfirmationEmail, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(resendConfirmationEmail.Email);
            if (user is null)
                return Result.Succes();

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));


            _logger.LogInformation("Confirmation Code {code}", code);

             await SendConfirmationEmail(user, code);

            return Result.Succes();
        }

        private async Task SendConfirmationEmail(ApplicationUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
                templateModel: new Dictionary<string, string>
                {
                { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
                }
            );

             BackgroundJob.Enqueue(()=> _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Email Confirmation", emailBody)) ;
             await Task.CompletedTask;

        }

        private async Task SendResetPasswordEmail(ApplicationUser user,string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
               templateModel: new Dictionary<string, string>
               {
                { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/forgetPassword?userId={user.Email}&code={code}" }
               }
           );

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Change Password", emailBody));
            await Task.CompletedTask;

        }

        private static string GenerateRefreshToken()=> Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<AuthResponse> GenerateAuthResponseAsync(ApplicationUser user)
        {
            // Generate JWT token
            var (Token, expire) = await generateToken(user);

            // Generate Refresh Token
            var refreshToken = GenerateRefreshToken();
            var ExpireDate = DateTime.UtcNow.AddDays(_refreshTokenExpire);

            // Save Refresh Token in user entity
            user.RefreshTokens.Add(new RefreshToken()
            {
                token = refreshToken,
                ExpireOn = ExpireDate
            });

            // Update user in DB
            await _userManager.UpdateAsync(user);

            // Return Auth Response
            var authResponse = new AuthResponse(
                Guid.NewGuid().ToString(),
                email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Token: Token,
                ExpireIn: expire,
                refreshToken: refreshToken,
                ExpireDate: ExpireDate
            );

            return authResponse;
        }


        private async Task<(string token, int expiresIn)> generateToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var rolesClaims = await _context.Roles.Join(
               _context.RoleClaims,
               r => r.Id, c => c.RoleId,
               (r, c) => new { r, c }).
               Where(rc => roles.Contains(rc.r.Name!)).
               Select(rc => rc.c.ClaimValue).
               Distinct().
               ToListAsync();

            return _jwtProvider.GenerateToken(user, roles, rolesClaims);
        }

    }
}
