using SurveyBasket.API.Contracts.Answers;

namespace SurveyBasket.API.Contracts.Question;

public record QuestionResponse(
    int Id,
    string Content,
    IEnumerable<AnswerResponse> answers
);
