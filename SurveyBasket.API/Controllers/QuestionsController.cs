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

        [HttpGet("JustGet")]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost("AddQuestion")]

        public async Task<IActionResult> Add([FromRoute]int PollId,QuestionRequest request,CancellationToken cancellationToken)
        {

            var result = await _questionService.AddAsync(PollId, request,cancellationToken);

            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { PollId, result.Value.Id }, result.Value) : result.ToProblem();

        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromRoute]int PollId)
        {
            var result = await _questionService.GetAll(PollId);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id,int PollId,CancellationToken cancellationToken)
        {
            var result = await _questionService.GetById(id, PollId, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

    }
}
