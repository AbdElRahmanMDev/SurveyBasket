using Microsoft.AspNetCore.Identity;
using SurveyBasket.API.Abstraction.Consts;
using SurveyBasket.API.Entities;

namespace SurveyBasket.API.Persistence.EntitiesConfigurations;

public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.OwnsMany(x => x.RefreshTokens).ToTable("RefreshTokens").WithOwner().HasForeignKey(foreignKeyPropertyNames:"UserId");

        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        builder.HasData(new ApplicationUser()
        {
            Id = DefaultUsers.AdminId,
            Email = DefaultUsers.AdminEmail,
            PasswordHash = passwordHasher.HashPassword(null!, DefaultUsers.AdminPassword),
            UserName = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
            EmailConfirmed = true,
            ConcurrencyStamp = DefaultUsers.AdminConCurrencyStamp,
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            FirstName = "SurveyBasket",
            LastName = "LastSurveyBasket"
        });
    }
}
