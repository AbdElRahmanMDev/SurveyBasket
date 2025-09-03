using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.API.Abstraction;
using SurveyBasket.API.Contracts.Authentication;
using SurveyBasket.API.Entities;
using SurveyBasket.API.Errors;
using SurveyBasket.API.Helpers;
using SurveyBasket.Authentication;
using System.ComponentModel.Design;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

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

        public AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider, ILogger<AuthService> logger,IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
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
            var (newToken, expire) = _jwtProvider.GenerateToken(user);

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
                return Result.Succes();
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

            await _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Email Confirmation", emailBody);
        }

        private static string GenerateRefreshToken()=> Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<AuthResponse> GenerateAuthResponseAsync(ApplicationUser user)
        {
            // Generate JWT token
            var (Token, expire) = _jwtProvider.GenerateToken(user);

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

    }
}
