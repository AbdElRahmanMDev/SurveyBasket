namespace SurveyBasket.API.Contracts.Authentication
{
    public record ConfirmEmailRequest(string UserId,string Code);


    public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
    {
        public ConfirmEmailRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
            RuleFor(x => x.Code)
                .NotEmpty();
        }
    }


}
