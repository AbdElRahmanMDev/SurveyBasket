namespace SurveyBasket.API.Services
{
    public interface IPollService
    {
        IEnumerable<Poll> GetAllPolls();

        Poll? GetpollById(int id);

        Poll  Add(Poll poll);

        bool Update(int id ,Poll poll);

        bool Delete(int id);
    }
}
