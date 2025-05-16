using SurveyBasket.API.Contracts.Question;

namespace SurveyBasket.API.Services
{
    public interface IQuestionService
    {
        Task<TResult<QuestionResponse>> AddAsync(int PollId,QuestionRequest request,CancellationToken cancellationToken=default);

        Task<TResult<IEnumerable<QuestionResponse>>> GetAll(int PollId,CancellationToken cancellationToken=default);

        Task<TResult<QuestionResponse>> GetById(int id, int pollId, CancellationToken cancellationToken = default);

    }
}
