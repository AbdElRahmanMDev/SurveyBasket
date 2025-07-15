using SurveyBasket.API.Contracts.Results;
using SurveyBasket.API.Contracts.Votes;
using SurveyBasket.API.Persistence;

namespace SurveyBasket.API.Services
{
    public class ResultService : IResultService
    {
        private readonly ApplicationDbContext _context;
        public ResultService(ApplicationDbContext context)
        {
            _context = context; 
        }
        public async Task<TResult<PollVoteResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken)
        {
            var response = await _context.Polls.
                Where(x=>x.Id== pollId).
                Select(x => new PollVoteResponse(
                    x.Title, x.Votes.Select(v =>
                    new VoteResponse(v.User.FirstName, v.SubmittedOn,
                    v.voteAnswers.Select(a =>
                    new QuestionAnswerResponse(a.Question.Content, a.Answer.Content)))))).SingleOrDefaultAsync();

            return response is null ?
                Result.Failure<PollVoteResponse>(PollErrors.PollNotFound) : Result.Succes(response);
            
        }


        public async Task<TResult<IEnumerable<VotesPerDayRespons>>> GetVotesPerDayAsync(int pollId,CancellationToken cancellationToken)
        {
           var pollIsExist= await _context.Polls.AnyAsync(x=>x.Id==pollId,cancellationToken);

            if(!pollIsExist)
                return Result.Failure<IEnumerable<VotesPerDayRespons>>(PollErrors.PollNotFound);

            var votesPerDay = await _context.Votes.Where(x => x.PollId == pollId)
                .GroupBy(x => new
                {
                    Date = DateOnly.FromDateTime(x.SubmittedOn)
                })
                .Select(x => new VotesPerDayRespons(x.Key.Date, x.Count()))
                .ToListAsync();




            return Result.Succes<IEnumerable<VotesPerDayRespons>>(votesPerDay); 

        }

        public async Task<TResult<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken)
        {
            var pollIsExist = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

            if (!pollIsExist)
                return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);

            var votesPerQuestion = await _context.VoteAnswers
             .Where(x => x.Vote.PollId == pollId)
             .Select(x => new VotesPerQuestionResponse(
                 x.Question.Content,
                 x.Question.voteAnswers
                     .GroupBy(x => new { AnswerId = x.Answer.Id, AnswerContent = x.Answer.Content })
                     .Select(g => new VotesPerAnswersResponse(
                         g.Key.AnswerContent,
                         g.Count()
                     ))
             ))
             .ToListAsync(cancellationToken);

            return Result.Succes<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
        }

    }
}
