using Microsoft.AspNetCore.Mvc.Routing;
using SurveyBasket.API.Contracts.Question;
using SurveyBasket.API.Contracts.Votes;
using SurveyBasket.API.Entites;
using SurveyBasket.API.Persistence;

namespace SurveyBasket.API.Services
{
    public class VoteService : IVoteService
    {
        private readonly ApplicationDbContext _context;

        public VoteService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result> AddVoteAsync(int pollId, string userId, VoteRequest voteRequest, CancellationToken cancellationToken)
        {
            var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId);

            if (!pollIsExists)
                return Result.Failure(PollErrors.PollNotFound);

            var voteIsExist= await _context.Votes.AnyAsync(x=>x.PollId==pollId &&  x.UserId==userId);

            if (voteIsExist)
                return Result.Failure(VoteError.DuplicatedVote);

            //to create vote

            var questionsIds= voteRequest.Answers.Select(x=>x.QuestionId).ToList();

            var pollquestions = await  _context.Questions.Where(x=>x.PollId==pollId).AllAsync(x=>questionsIds.Contains(x.Id));    

            if(pollquestions is false)
                return Result.Failure(VoteError.InvalidQuestions);


            var vote = new Vote()
            {
                PollId = pollId,
                UserId = userId,
                voteAnswers = voteRequest.Answers.Adapt<IEnumerable<VoteAnswers>>().ToList()
            };


            await _context.Votes.AddAsync(vote);


            _context.SaveChanges();


            return Result.Succes();
        }

    }
}
