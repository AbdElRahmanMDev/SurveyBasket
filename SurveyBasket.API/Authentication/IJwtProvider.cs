
namespace SurveyBasket.Authentication;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(ApplicationUser user);

    string? ValidateJwt(string token);
}