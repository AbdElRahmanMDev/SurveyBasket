namespace SurveyBasket.API.Errors;

public class QuestionErrors
{
    public static readonly Error QuestionAlreadyExist =
                    new Error("Question.AlreadyExist", "Duplicate question is Not Allowed");

    public static readonly Error QuestionNotFound =
                new Error("Question.NotFound", "question is Not Found");
}
