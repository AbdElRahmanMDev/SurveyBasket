using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.API.ValidationsAttributes
{
    public class MinAgeAttribute : ValidationAttribute
    {
        int minAge;
        public MinAgeAttribute(int minAge)
        {
            this.minAge = minAge;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                var date = (DateTime)value;

                if (DateTime.Today < date.AddYears(minAge))
                    return new ValidationResult(ErrorMessage = ErrorMessage??$"Default Error Message {minAge}");
            }
            return ValidationResult.Success;
        }
      

    }
}
