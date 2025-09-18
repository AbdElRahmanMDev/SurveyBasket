namespace SurveyBasket.API.Contracts.Users
{
    public record UpdateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        IList<string> Roles
        );


    class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Email).
               EmailAddress().
               NotEmpty();

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
