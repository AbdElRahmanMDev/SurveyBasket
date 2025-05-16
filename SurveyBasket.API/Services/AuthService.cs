using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.API.Abstraction;
using SurveyBasket.API.Contracts.Authentication;
using SurveyBasket.API.Entities;
using SurveyBasket.API.Errors;
using SurveyBasket.Authentication;
using System.Reflection;
using System.Security.Cryptography;

namespace SurveyBasket.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtProvider _jwtProvider;

        private readonly int _refreshTokenExpire = 14;
        public AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
        }


        public async Task<TResult<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken token = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials); ;

            var IsvalidaPassword =await _userManager.CheckPasswordAsync(user, password);

            if (!IsvalidaPassword)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            var (Token, expire) = _jwtProvider.GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            var ExpireDate = DateTime.UtcNow.AddDays(_refreshTokenExpire);
            user.RefreshTokens.Add(new RefreshToken()
            {
                token = refreshToken,
                ExpireOn = ExpireDate
            });

            await _userManager.UpdateAsync(user);
            
            var authResponse= new AuthResponse(Guid.NewGuid().ToString(), email: "Test@gmail.com", FirstName: "abdo", LastName: "abdoo",
                Token: Token,
                ExpireIn: expire,
                refreshToken: refreshToken,
                ExpireDate: ExpireDate);

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

        private static string GenerateRefreshToken()=> Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

      
    }
}
