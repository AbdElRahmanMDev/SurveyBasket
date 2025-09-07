using SurveyBasket.API.Contracts.Authentication;
using SurveyBasket.API.Contracts.Users;

namespace SurveyBasket.API.Services
{
    public interface IAuthService
    {
        Task<TResult<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken token = default);
        Task<TResult<AuthResponse>> GenerateNewTokenAndRefreshToken(string _token, string refreshToken,CancellationToken token=default);
        Task<Result> Revoked(string _token, string refreshToken,CancellationToken token=default);

        Task<Result> RegisterAsync(SurveyBasket.API.Authentication.RegisterRequest request, CancellationToken cancellationToken = default);

        Task<Result> ConfirmCode(ConfirmEmailRequest confirmEmailRequest, CancellationToken cancellationToken = default);

        Task<Result> ResendConfirmCode(ResendConfirmationEmail resendConfirmationEmail, CancellationToken cancellationToken = default);

        Task<Result> ForgetPassword(ForgetPasswordRequest request);

        Task<Result> ResetPassword(SurveyBasket.API.Authentication.ResetPasswordRequest request);




    }
}
