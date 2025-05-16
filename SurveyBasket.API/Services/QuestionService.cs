using Microsoft.AspNetCore.Authorization;
using SurveyBasket.API.Contracts.Answers;
using SurveyBasket.API.Contracts.Question;
using SurveyBasket.API.Entites;
using SurveyBasket.API.Persistence;

namespace SurveyBasket.API.Services;

public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _context;


    public QuestionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TResult<QuestionResponse>> AddAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == PollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExists = await _context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == PollId, cancellationToken: cancellationToken);

        if (questionIsExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionAlreadyExist);

        var question = request.Adapt<Question>();
        question.PollId = PollId;

        await _context.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Succes(question.Adapt<QuestionResponse>());
    }

    public async Task<TResult<IEnumerable<QuestionResponse>>> GetAll(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
             .Where(x => x.PollId == pollId)
             .Include(x => x.answers) // 👈 match property case!
             .ProjectToType<QuestionResponse>()
             .AsNoTracking()
             .ToListAsync(cancellationToken);

        return Result.Succes<IEnumerable<QuestionResponse>>(questions);

    }

    public async Task<TResult<QuestionResponse>> GetById(int id, int pollId,  CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var question = await _context.Questions.Include(x=>x.answers).SingleOrDefaultAsync(x=>x.Id == id && x.PollId==pollId);

        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Succes<QuestionResponse>(question.Adapt<QuestionResponse>());
        

    }
}
