namespace SurveyBasket.API.Services
{
    public interface IPollService
    {
      Task<IEnumerable<Poll>> GetAllPollsAsync(CancellationToken cancellationToken = default);

        Task<TResult<PollResponse>> GetpollByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<TResult<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default);

        Task<Result> UpdateAysnc(int id, Poll poll, CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<Result> ToggleStatusAsync(int id, CancellationToken cancellationToken = default);

    }
}
