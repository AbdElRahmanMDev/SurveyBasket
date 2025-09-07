using SurveyBasket.API.Abstraction.Consts;

namespace SurveyBasket.API.Authentication
{
    public record ResetPasswordRequest(
        string Email,
        string Code,
        string NewPassword        
        );


    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.NewPassword)
                            .NotEmpty()
                            .Matches(RegexPatterns.PasswordPattern)
                            .WithMessage("Password must be at least 6 characters long and contain at least one uppercase, one lowercase, one digit, and one special character.");

            RuleFor(x => x.Code)
                .NotEmpty();

        }
    }


}
