using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using SurveyBasket.API.Abstraction;
using SurveyBasket.API.Contracts.Polls;

namespace SurveyBasket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController : ControllerBase
{
    private readonly IPollService _pollService;
    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }



    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls=await _pollService.GetAllPollsAsync(cancellationToken);
        var responses=polls.Adapt<IEnumerable<PollResponse>>();
        return Ok(responses);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetpollByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);

    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest pollRequest, CancellationToken cancellationToken)
    {
        var newPoll = await _pollService.AddAsync(pollRequest, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = newPoll.Value.Id }, newPoll);

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollmodel, CancellationToken cancellationToken)
    {

        var poll = pollmodel.Adapt<Poll>();
        var result = await _pollService.UpdateAysnc(id, poll, cancellationToken);
        
        return result.IsSuccess ? NoContent() : result.ToProblem(StatusCodes.Status404NotFound);


    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);
        
        return result.IsSuccess? NoContent(): result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpPut("{id}/TogglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.ToggleStatusAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem(statusCode: StatusCodes.Status404NotFound);
    }
}
