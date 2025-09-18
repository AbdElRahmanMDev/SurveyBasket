using SurveyBasket.API.Abstraction.Consts;

namespace SurveyBasket.API.Controllers
{
    public record CreateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        IList<string> Roles

        );


    class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
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
                .Length(min: 3, max: 100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(min: 3, max: 100);


            RuleFor(x => x.Roles)
                .Must(x => x.Distinct().Count() == x.Count()).WithMessage("You Can not add duplicated role for the same user").
                When(x => x.Roles != null);


        }
    }


}
