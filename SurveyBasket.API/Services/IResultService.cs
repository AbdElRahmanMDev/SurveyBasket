using SurveyBasket.API.Contracts.Results;
using SurveyBasket.API.Contracts.Votes;

namespace SurveyBasket.API.Services
{
    public interface IResultService
    {
        public  Task<TResult<PollVoteResponse>> GetPollVotesAsync(int pollId,CancellationToken cancellationToken);

        public Task<TResult<IEnumerable<VotesPerDayRespons>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken);

        public Task<TResult<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken);


    }
}
