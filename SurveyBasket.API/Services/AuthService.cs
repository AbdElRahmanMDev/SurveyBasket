using Microsoft.AspNetCore.Identity;
using SurveyBasket.API.Contracts.Authentication;
using SurveyBasket.API.Entities;
using SurveyBasket.Authentication;

namespace SurveyBasket.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtProvider _jwtProvider;
        public AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
        }


        public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken token = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user is null)
                return null;

            var IsvalidaPassword =await _userManager.CheckPasswordAsync(user, password);

            if(!IsvalidaPassword)
                return null;

            var (Token, expire) = _jwtProvider.GenerateToken(user);

            return new AuthResponse(Guid.NewGuid().ToString(), email: "Test@gmail.com", FirstName: "abdo", LastName: "abdoo", 
                Token: Token,
                ExpireIn:expire);
        }
    }
}
