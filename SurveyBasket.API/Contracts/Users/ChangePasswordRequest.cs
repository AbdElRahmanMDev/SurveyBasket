using SurveyBasket.API.Abstraction.Consts;

namespace SurveyBasket.API.Contracts.Users
{
    public record ChangePasswordRequest(
        string currentPassword,
        string newPassword
        );

    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.currentPassword)
                .NotEmpty()
                .NotEqual(x => x.newPassword);

            RuleFor(x => x.newPassword)
                 .NotEmpty()
                 .Matches(RegexPatterns.PasswordPattern)
                 .WithMessage("Password must be at least 6 characters long and contain at least one uppercase, one lowercase, one digit, and one special character.");


        }
    }


}
