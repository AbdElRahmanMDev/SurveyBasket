namespace SurveyBasket.API.Services
{
    public interface IPollService
    {
      Task<IEnumerable<Poll>> GetAllPollsAsync(CancellationToken cancellationToken = default);

        Task<Poll?> GetpollByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default);

        Task<bool> UpdateAysnc(int id, Poll poll, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> ToggleStatusAsync(int id, CancellationToken cancellationToken = default);
    }
}
