namespace SurveyBasket.API.Contracts.Authentication
{
    public record ForgetPasswordRequest(
        string Email
        );
   

    public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
    {
        public ForgetPasswordRequestValidator()
        {
            RuleFor(x => x.Email).
                    EmailAddress().
                    NotEmpty();


        }
    }
}
