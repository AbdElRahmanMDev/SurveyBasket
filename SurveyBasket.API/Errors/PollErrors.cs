

namespace SurveyBasket.API.Errors
{
    public class PollErrors
    {
        public static readonly Error PollNotFound =
                            new Error("Poll.NotFound", "No poll was found with the given ID");

        public static readonly Error PollAlreadyExist =
                         new Error("Poll.AlreadyExist", "Duplicate Poll is Not Allowed");
    }
}
