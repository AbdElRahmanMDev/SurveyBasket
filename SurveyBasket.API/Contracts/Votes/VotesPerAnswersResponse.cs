namespace SurveyBasket.API.Contracts.Votes
{
    public record VotesPerAnswersResponse(
        string Answer,
        int Count
        );
    
}
