namespace SurveyBasket.API.Contracts.Authentication
{
    public record ResendConfirmationEmail(string Email);

    public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmail>
    {
        public ResendConfirmationEmailValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
                
        }
    }


}
