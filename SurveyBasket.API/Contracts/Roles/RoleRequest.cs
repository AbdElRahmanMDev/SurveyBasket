namespace SurveyBasket.API.Contracts.Roles
{
    public record RoleRequest(
        string Name,
         IList<string> Permissions
);


    class RoleRequestValidator : AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .Length(3, 200);
            RuleFor(x => x.Permissions)
                .NotNull().WithMessage("Permissions are required.")
                .Must(p => p.Distinct().Count() == p.Count()).WithMessage("At least one permission must be specified.")
                .When(x => x.Permissions != null);
        }
    }

}
