using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SurveyBasket.API.Authentication.Filters
{
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;
        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy=await base.GetPolicyAsync(policyName);

            if (policy is not null)
                return policy;

            var newPolicy = new AuthorizationPolicyBuilder().AddRequirements(new PermissionsRequirement(policyName)).Build();

            _options.AddPolicy(policyName, newPolicy);

            return newPolicy;

        }
    }
}
