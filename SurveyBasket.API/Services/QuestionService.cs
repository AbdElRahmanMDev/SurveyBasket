using Microsoft.AspNetCore.Authorization;
using SurveyBasket.API.Contracts.Answers;
using SurveyBasket.API.Contracts.Question;
using SurveyBasket.API.Entites;
using SurveyBasket.API.Persistence;

namespace SurveyBasket.API.Services;

public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<QuestionService> _logger;
    public QuestionService(ApplicationDbContext context, ICacheService cacheService, ILogger<QuestionService> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
        
    }

    public async Task<TResult<QuestionResponse>> AddAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == PollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExists = await _context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == PollId, cancellationToken: cancellationToken);

        if (questionIsExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

        var question = request.Adapt<Question>();
        question.PollId = PollId;

        await _context.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _cacheService.Remove($"poll_{PollId}_questions", cancellationToken);

        return Result.Succes(question.Adapt<QuestionResponse>());
    }

    public async Task<TResult<IEnumerable<QuestionResponse>>> GetAll(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        IEnumerable<QuestionResponse> questionResponses = [];

        var cachedQuestions =await _cacheService.GetAsync<IEnumerable<QuestionResponse>>($"poll_{pollId}_questions", cancellationToken);
        if(cachedQuestions is null)
        {
            _logger.LogInformation("Select Question from Database");
            questionResponses = await _context.Questions
                .Where(x => x.PollId == pollId)
                .Select(x => new QuestionResponse(x.Id, x.Content, x.answers.Select(a => new AnswerResponse(a.Id, a.Content))))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            await _cacheService.SetAsync($"poll_{pollId}_questions", questionResponses, cancellationToken);
        }
        else
        {
            _logger.LogInformation("Select Question from Cache");
            questionResponses = cachedQuestions;
        }

        
        return Result.Succes<IEnumerable<QuestionResponse>>(questionResponses);

    }

    public async Task<TResult<QuestionResponse>> GetById(int id, int pollId,  CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var question = await _context.Questions.Where(x=>x.Id==id &&  x.PollId == pollId).ProjectToType<QuestionResponse>()
            .SingleOrDefaultAsync();

        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Succes<QuestionResponse>(question.Adapt<QuestionResponse>());
        

    }

    public async Task<TResult<IEnumerable<QuestionResponse>>> GetAvailableAsync(int PollId,string UserId,CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == PollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var VoteIsExist = await _context.Votes.AnyAsync(x => x.PollId == PollId && x.UserId == UserId);
        if (VoteIsExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteError.DuplicatedVote);

       var questions= await _context.Questions
            .Where(x=>x.PollId==PollId && x.IsActive)
            .Select(x=> new QuestionResponse(x.Id,x.Content,x.answers.Select(a=>new AnswerResponse(a.Id,a.Content))))
            .AsNoTracking()
            .ToListAsync(cancellationToken);  
        return Result.Succes<IEnumerable<QuestionResponse>>(questions);  

    }

    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var questionIsExists = await _context.Questions
                   .AnyAsync(x => x.PollId == pollId
                       && x.Id != id
                       && x.Content == request.Content,
                       cancellationToken
                   );

        if (questionIsExists)
            return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

        var question = await _context.Questions
            .Include(x => x.answers)
            .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        //current answers
        var currentAnswers = question.answers.Select(x => x.Content).ToList();

        //add new answer
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(answer =>
        {
            question.answers.Add(new Answer { Content = answer });
        });

        question.answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content);
        });

        await _context.SaveChangesAsync(cancellationToken);

        await _cacheService.Remove($"poll_{pollId}_questions", cancellationToken);

        return Result.Succes();
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        await _cacheService.Remove($"poll_{pollId}_questions", cancellationToken);

        return Result.Succes();
    }
}
