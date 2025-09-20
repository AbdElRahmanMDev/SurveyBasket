
using Hangfire;
using SurveyBasket.API.Persistence;

namespace SurveyBasket.API.Services
{
    public class PollService : IPollService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        public PollService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }



        public async Task<IEnumerable<PollResponse>> GetAllPollsAsync(CancellationToken cancellationToken = default)
        {
            var poll = await _context.Polls
                .ProjectToType<PollResponse>()
                .AsNoTracking().ToListAsync(cancellationToken);

            return poll;
        }




        public async Task<TResult<PollResponse>> GetpollByIdAsync(int id, CancellationToken cancellationToken = default)
        {

            var poll = await _context.Polls.FindAsync(id, cancellationToken);
            var response = poll.Adapt<PollResponse>();

            return response is null ? Result.Failure<PollResponse>(PollErrors.PollNotFound) : Result.Succes(response);

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


            var Exist = await _context.Polls.AnyAsync(x => x.Title == request.Title);
            if (Exist)
                return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);
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

            if (poll.IsPublished && poll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                BackgroundJob.Enqueue(() => _notificationService.SendNewPollNotification(poll.Id));

            return Result.Succes();
        }

        public async Task<IEnumerable<PollResponse>> GetCurrentAsyncV1(CancellationToken cancellationToken = default)
        {
            var polls = await _context.Polls.
                Where(x => x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && DateOnly.FromDateTime(DateTime.UtcNow) <= x.EndsAt)
                .ProjectToType<PollResponse>()
                .AsNoTracking().ToListAsync(cancellationToken);

            return polls;
        }

        public async Task<IEnumerable<PollResponseV2>> GetCurrentAsyncV2(CancellationToken cancellationToken = default)
        {
            var polls = await _context.Polls.
                Where(x => x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && DateOnly.FromDateTime(DateTime.UtcNow) <= x.EndsAt)
                .ProjectToType<PollResponseV2>()
                .AsNoTracking().ToListAsync(cancellationToken);

            return polls;
        }
    }
}
