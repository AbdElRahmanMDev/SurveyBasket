namespace SurveyBasket.API.Services
{
    public interface IPollService
    {
        Task<IEnumerable<PollResponse>> GetAllPollsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<PollResponse>> GetCurrentAsyncV1(CancellationToken cancellationToken = default);
        Task<IEnumerable<PollResponseV2>> GetCurrentAsyncV2(CancellationToken cancellationToken = default);

        Task<TResult<PollResponse>> GetpollByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<TResult<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default);

        Task<Result> UpdateAysnc(int id, Poll poll, CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<Result> ToggleStatusAsync(int id, CancellationToken cancellationToken = default);

    }
}
