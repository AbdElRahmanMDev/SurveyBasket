using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.API.Contracts.Question;

namespace SurveyBasket.API.Controllers
{
    [Route("api/Polls/{PollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost]

        public async Task<IActionResult> Add([FromRoute]int PollId,QuestionRequest request,CancellationToken cancellationToken)
        {

            var result = await _questionService.AddAsync(PollId, request,cancellationToken);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(Get), new { PollId, result.Value.Id }, result.Value);

            return result.Error.Equals(QuestionErrors.QuestionAlreadyExist) ?
                result.ToProblem(StatusCodes.Status409Conflict) :
                result.ToProblem(StatusCodes.Status404NotFound);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromRoute]int PollId)
        {
            var result = await _questionService.GetAll(PollId);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id,int PollId,CancellationToken cancellationToken)
        {
            var result = await _questionService.GetById(id, PollId, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
        }

    }
}
