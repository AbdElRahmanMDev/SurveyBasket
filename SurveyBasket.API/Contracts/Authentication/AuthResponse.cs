namespace SurveyBasket.API.Contracts.Authentication
{
    public record AuthResponse(
        string id,
        string? email,
        string FirstName,
        string LastName,
        string Token,
        int ExpireIn ,
        string refreshToken,
        DateTime ExpireDate
        );
    
}
