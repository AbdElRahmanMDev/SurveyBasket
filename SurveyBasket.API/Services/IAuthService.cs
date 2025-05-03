using SurveyBasket.API.Contracts.Authentication;

namespace SurveyBasket.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken token = default);
        
    }
}
