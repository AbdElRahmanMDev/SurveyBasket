using SurveyBasket.API.Abstraction;

namespace SurveyBasket.API.Errors
{
    public class UserErrors
    {
        public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidJwtToken =
            new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidRefreshToken =
            new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

        public static readonly Error DuplicateEmail =
            new("User.DuplicateEmail", "Another User with the same Email", StatusCodes.Status409Conflict);

        public static readonly Error EmailNotConfirmed=
            new("User.EmailNotConfirmed", "Email not confirmed", StatusCodes.Status403Forbidden);

        public static readonly Error InvalidCode=
            new("User.InValidCode", "InValid code", StatusCodes.Status401Unauthorized);


        public static readonly Error DuplicatedConfirmation=
            new("User.DuplicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);


    }
}
