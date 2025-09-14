namespace SurveyBasket.API.Errors
{
    public static class RoleErrors
    {
        public static readonly Error RoleNotFound = new Error("RoleNotFound", "Role not found", StatusCodes.Status404NotFound);


        public static readonly Error RoleAlreadyExists = new Error("RoleAlreadyExists", "Role already exists", StatusCodes.Status409Conflict);

        public static readonly Error InvalidPermission = new Error("InvalidPermission", "Invalid permission", StatusCodes.Status400BadRequest);
    }
}
