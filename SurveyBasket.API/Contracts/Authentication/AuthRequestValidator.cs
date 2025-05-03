namespace SurveyBasket.API.Contracts.Authentication;

public class AuthRequestValidator : AbstractValidator<AuthRequest>
{
    public AuthRequestValidator()
    {

        RuleFor(x => x.email).
            EmailAddress().
            NotEmpty();

        RuleFor(x => x.password)
            .NotEmpty();
    }

 
}
