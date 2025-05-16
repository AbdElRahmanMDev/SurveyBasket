using System.Security.Cryptography;

namespace SurveyBasket.API.Contracts.Polls
{
    public class PollRequestValidator : AbstractValidator<PollRequest>
    {
        public PollRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(errorMessage: "Please Add a Title")
                .Length(min: 3, max: 10);

            RuleFor(x => x.Summary)
                .NotEmpty()
                .Length(min: 3, max: 1500);

            RuleFor(x => x.StartsAt)
                .NotEmpty()
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
          
            RuleFor(x => x.EndsAt)
                .NotEmpty();
            RuleFor(x => x)
                .Must(HasValidDate)
                .WithName(nameof(PollRequest.EndsAt))
                .WithMessage("{PropertyName} must be greater than or Equal start Date");

        }

        private bool HasValidDate(PollRequest pollRequest)
        {
            return pollRequest.EndsAt >= pollRequest.StartsAt;
        }
    }
}
