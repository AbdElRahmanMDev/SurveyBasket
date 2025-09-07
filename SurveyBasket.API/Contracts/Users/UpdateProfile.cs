using SurveyBasket.API.Abstraction.Consts;

namespace SurveyBasket.API.Contracts.Users
{
    public record UpdateProfile(
        string FirstName,
        string LastName
        );
    
    public class UpdateProfileValidator : AbstractValidator<UpdateProfile>
    {
        public UpdateProfileValidator()
        {

            
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(min: 3, max: 100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(min: 3, max: 100);
        }
    }
}
