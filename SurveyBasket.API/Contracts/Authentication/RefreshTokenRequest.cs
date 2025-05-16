namespace SurveyBasket.API.Contracts.Authentication
{
    public record RefreshTokenRequest(
        string token,
        string refreshToken
        );
    
}
