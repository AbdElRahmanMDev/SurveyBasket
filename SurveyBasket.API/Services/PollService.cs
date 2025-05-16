
using SurveyBasket.API.Abstraction;
using SurveyBasket.API.Persistence;
using System.Runtime.ExceptionServices;

namespace SurveyBasket.API.Services
{
    public class PollService : IPollService
    {
        private readonly ApplicationDbContext _context;

        public PollService(ApplicationDbContext context)
        {
            _context = context;
        }

       

        public async Task<IEnumerable<Poll>> GetAllPollsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
        }




        public async Task<TResult<PollResponse>> GetpollByIdAsync(int id, CancellationToken cancellationToken = default)
        {

            var poll = await _context.Polls.FindAsync(id, cancellationToken);
            var response = poll.Adapt<PollResponse>();

            return response is null ? Result.Failure<PollResponse>(PollErrors.PollNotFound) :Result.Succes(response);

        }

        public async Task<Result> UpdateAysnc(int id, Poll poll, CancellationToken cancellationToken = default)
        {
            var currentpoll = await _context.Polls.FindAsync(id, cancellationToken);
          
            if (currentpoll is null)
                return Result.Failure(PollErrors.PollNotFound);

            currentpoll.Summary = poll.Summary;
            currentpoll.Title = poll.Title;
            currentpoll.EndsAt = poll.EndsAt;
            currentpoll.StartsAt = poll.StartsAt;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Succes(); 

        }
        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await _context.Polls.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (poll is null)
                return Result.Failure(PollErrors.PollNotFound);

            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Succes();

        }

        public async Task<TResult<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
        {


          var Exist=await _context.Polls.AnyAsync(x => x.Title == request.Title);
            if (Exist)
                return Result.Failure<PollResponse>(PollErrors.PollAlreadyExist);
            var poll = request.Adapt<Poll>();



            await _context.AddAsync(poll, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Succes(poll.Adapt<PollResponse>());
        }


        public async Task<Result> ToggleStatusAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await _context.Polls.FindAsync(id, cancellationToken);
            if (poll is null) return Result.Failure(PollErrors.PollNotFound);

            poll.IsPublished = !poll.IsPublished;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Succes();
        }

        
    }
}
