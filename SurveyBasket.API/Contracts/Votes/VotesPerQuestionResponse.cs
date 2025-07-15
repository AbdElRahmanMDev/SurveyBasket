namespace SurveyBasket.API.Contracts.Votes
{
    public record VotesPerQuestionResponse(string Question ,IEnumerable<VotesPerAnswersResponse> SelectedAnswers );
    
}
