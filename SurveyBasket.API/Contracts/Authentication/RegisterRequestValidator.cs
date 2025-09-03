using SurveyBasket.API.Abstraction.Consts;

namespace SurveyBasket.API.Authentication
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {

            RuleFor(x => x.Email).
                EmailAddress().
                NotEmpty();

            RuleFor(x => x.Password)
                 .NotEmpty()
                 .Matches(RegexPatterns.PasswordPattern)
                 .WithMessage("Password must be at least 6 characters long and contain at least one uppercase, one lowercase, one digit, and one special character.");


            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(min:3,max:100);

            RuleFor(x=>x.LastName)
                .NotEmpty()
                .Length(min: 3, max: 100);
        }
    }
}
