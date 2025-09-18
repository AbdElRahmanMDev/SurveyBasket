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

        public static readonly Error EmailNotConfirmed =
            new("User.EmailNotConfirmed", "Email not confirmed", StatusCodes.Status403Forbidden);

        public static readonly Error InvalidCode =
            new("User.InValidCode", "InValid code", StatusCodes.Status401Unauthorized);


        public static readonly Error DuplicatedConfirmation =
            new("User.DuplicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);

        public static readonly Error DuplicatedEmail =
           new("User.DuplicatedEmail", "Email already Exist", StatusCodes.Status400BadRequest);

        public static readonly Error InvalidRoles = new Error("InvalidRoles", "InvalidRoles", StatusCodes.Status400BadRequest);


        public static readonly Error UserIsDisabled =
            new("User.Disabled", "User is Disabled,Please Contaxt your Adminstrator", StatusCodes.Status401Unauthorized);


        public static readonly Error LockedUsers =
            new("User.LockedUsers", "Locked User,Please Contaxt your Adminstrator", StatusCodes.Status401Unauthorized);

        public static readonly Error UserNotFound =
            new("User.NotFound", "User is Not Found", StatusCodes.Status404NotFound);

    }
}
