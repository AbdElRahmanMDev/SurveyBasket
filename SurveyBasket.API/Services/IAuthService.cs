using SurveyBasket.API.Contracts.Authentication;

namespace SurveyBasket.API.Services
{
    public interface IAuthService
    {
        Task<TResult<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken token = default);
        Task<TResult<AuthResponse>> GenerateNewTokenAndRefreshToken(string _token, string refreshToken,CancellationToken token=default);
        Task<Result> Revoked(string _token, string refreshToken,CancellationToken token=default);
    }
}
