using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.API.Authentication.Filters
{
    public class PermissionsRequirement : IAuthorizationRequirement
    {
        public string permission { get; }
        public PermissionsRequirement(string permission)
        {
            this.permission = permission;
        }
    }
}
