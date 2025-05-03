
using SurveyBasket.API.Persistence;

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




        public async Task<Poll?> GetpollByIdAsync(int id, CancellationToken cancellationToken = default)
        {

            var poll = await _context.Polls.FindAsync(id, cancellationToken);
            return poll is null ? null : poll;

        }

        public async Task<bool> UpdateAysnc(int id, Poll poll, CancellationToken cancellationToken = default)
        {
            var currentpoll = await GetpollByIdAsync(id, cancellationToken);
            if (currentpoll is null)
                return false;

            currentpoll.Summary = poll.Summary;
            currentpoll.Title = poll.Title;
            currentpoll.EndsAt = poll.EndsAt;
            currentpoll.StartsAt= poll.StartsAt;
           await _context.SaveChangesAsync(cancellationToken);

            return true;

        }
        public  async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await _context.Polls.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (poll is null)
                return false;

            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync(cancellationToken);

            return true;

        }

        public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default)
        {
            await  _context.Polls.AddAsync(poll,cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return poll;
        }

      
        public async Task<bool> ToggleStatusAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await GetpollByIdAsync(id, cancellationToken);
            if (poll is null) return false;

            poll.IsPublished = !poll.IsPublished;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
