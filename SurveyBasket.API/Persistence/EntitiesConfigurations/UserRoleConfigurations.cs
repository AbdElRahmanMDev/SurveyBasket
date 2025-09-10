using Microsoft.AspNetCore.Identity;
using SurveyBasket.API.Abstraction.Consts;

namespace SurveyBasket.API.Persistence.EntitiesConfigurations
{
    public class UserRoleConfigurations : IEntityTypeConfiguration<IdentityUserRole<string>>

    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(new IdentityUserRole<string>
            {
                RoleId = DefaultRoles.AdminRoleId,
                UserId = DefaultUsers.AdminId,
            });
        }
    }
}
