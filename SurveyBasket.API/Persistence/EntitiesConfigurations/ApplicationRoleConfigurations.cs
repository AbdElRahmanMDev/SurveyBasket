
using SurveyBasket.API.Abstraction.Consts;

namespace SurveyBasket.API.Persistence.EntitiesConfigurations
{
    public class ApplicationRoleConfigurations : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData(new ApplicationRole()
            {
                Id = DefaultRoles.AdminRoleId,
                Name = DefaultRoles.AdminRoleName,
                NormalizedName = DefaultRoles.AdminRoleName.ToUpper(),
                ConcurrencyStamp = DefaultRoles.AdminConCurrencyStamp,
                IsDefault = false

            },
            new ApplicationRole()
            {
                Id=DefaultRoles.MemberRoleId,
                Name=DefaultRoles.MemberRoleName,
                NormalizedName = DefaultRoles.MemberRoleName.ToUpper(),
                ConcurrencyStamp = DefaultRoles.MemberConCurrencyStamp,
                IsDefault = true    
            });

            
        }
    }
}
