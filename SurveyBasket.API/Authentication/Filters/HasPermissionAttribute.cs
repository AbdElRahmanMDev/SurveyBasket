using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.API.Authentication.Filters
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission) : base(policy: permission)
        {
        }

    }
}
