namespace SurveyBasket.API.Contracts.Question;

public record QuestionRequest(string Content, List<string> Answers);

