namespace SurveyBasket.API.Contracts.Results
{
    public record PollVoteResponse(
        string Title,
        IEnumerable<VoteResponse> VoteResponses
        );
    
}
