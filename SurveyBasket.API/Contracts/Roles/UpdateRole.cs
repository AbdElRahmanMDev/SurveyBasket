namespace SurveyBasket.API.Contracts.Roles
{
    public record UpdateRole
    (
        string RoleId,
        string Name,
        IEnumerable<string> Permissions
    );

    class UpdateRoleValidation : AbstractValidator<UpdateRole>
    {
        public UpdateRoleValidation()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is Required");
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
