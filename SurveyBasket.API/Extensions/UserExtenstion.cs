using System.Security.Claims;

namespace SurveyBasket.API.Extensions
{
    public static class UserExtenstion
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
