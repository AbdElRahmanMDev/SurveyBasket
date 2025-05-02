namespace SurveyBasket.API.Contracts.Validations
{
    public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
    {
        public CreatePollRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(errorMessage:"Please Add a Title")
                .Length(min:3,max:10);
        }
    }
}
