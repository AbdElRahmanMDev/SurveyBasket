namespace SurveyBasket.API.Contracts.Students
{
    public class StudentValidator : AbstractValidator<Student>
    {
        public StudentValidator()
        {
            RuleFor(x => x.DateOfBirth)
                .Must(BeMoreThan18Years)
                .When(x => x.DateOfBirth.HasValue)
                .WithMessage("{PropertyName} is Invalid , age should be 18 years At Least");
        }

        private bool BeMoreThan18Years(DateTime? dateOfBirth)
        {
            return DateTime.Today > dateOfBirth!.Value.AddYears(18);
        }

    }
}
