using Microsoft.AspNetCore.Authorization;
using SurveyBasket.API.Abstraction.Consts;

namespace SurveyBasket.API.Authentication.Filters
{
    public class PermissionRequirementHandler : AuthorizationHandler<PermissionsRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsRequirement requirement)
        {
            var user = context.User.Identity;
            if (user is null || !user.IsAuthenticated)
                return;

            var hasPermission=context.User.Claims.Any(c=>c.Type == Permissions.Type && c.Value == requirement.permission);
            if (!hasPermission)
                return;

            context.Succeed(requirement);
            return;

        }
    }
}
