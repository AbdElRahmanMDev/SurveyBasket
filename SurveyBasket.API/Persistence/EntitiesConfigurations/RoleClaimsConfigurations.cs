using Microsoft.AspNetCore.Identity;
using SurveyBasket.API.Abstraction.Consts;

namespace SurveyBasket.API.Persistence.EntitiesConfigurations
{
    public class RoleClaimsConfigurations : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            var permissions= Permissions.GetAllPermissons();
            var adminClaims = new List<IdentityRoleClaim<string>>();
            for (var i = 0; i <permissions.Count; i++)
            {
                adminClaims.Add(new IdentityRoleClaim<string>()
                {
                    Id = i + 1,
                    ClaimType = Permissions.Type,
                    ClaimValue = permissions[i],
                    RoleId = DefaultRoles.AdminRoleId
                });

            }

            builder.HasData(adminClaims);
          
        }
    }
}
