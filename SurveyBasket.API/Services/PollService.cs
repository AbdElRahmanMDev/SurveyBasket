
namespace SurveyBasket.API.Services
{
    public class PollService : IPollService
    {
        public static List<Poll> _polls = new List<Poll>()
        {
            new Poll()
            {
                Id = 1,
                Description="First Poll added",
                Title="Poll 1"
            }
        };

        public Poll Add(Poll poll)
        {
            poll.Id = _polls.Count + 1;
            _polls.Add(poll);
            return poll;
        }

     
        public IEnumerable<Poll> GetAllPolls()
        {
            return _polls;
        }

        public Poll? GetpollById(int id)
        {

            var poll = _polls.SingleOrDefault(x => x.Id == id);
            return poll is null? null : poll;

        }

        public bool Update(int id,Poll poll)
        {
            var  currentpoll= GetpollById(id);
            if (currentpoll is null)
                return false;

            currentpoll.Title= poll.Title;
            currentpoll.Description= poll.Description;

            return true;

        }
        public bool Delete(int id)
        {
            var poll=_polls.FirstOrDefault(x => x.Id == id);

            if (poll is null)
                return false;

            _polls.Remove(poll);

            return true;

        }


    }
}
