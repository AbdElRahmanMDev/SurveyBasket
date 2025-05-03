using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
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
        var poll = await _pollService.GetpollByIdAsync(id, cancellationToken);

        if (poll is null)
            return NotFound();

        var response = poll.Adapt<PollResponse>();
        return Ok(response);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest pollRequest, CancellationToken cancellationToken)
    {
        var poll = pollRequest.Adapt<Poll>();
        var newPoll = await _pollService.AddAsync(poll, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = newPoll.Id }, newPoll);

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollmodel, CancellationToken cancellationToken)
    {

        var poll = pollmodel.Adapt<Poll>();
        var isUpdated = await _pollService.UpdateAysnc(id, poll, cancellationToken);

        if (!isUpdated)
            return NotFound();

        return NoContent();

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isDeleted = await _pollService.DeleteAsync(id, cancellationToken);

        if (!isDeleted)
            return NotFound();

        return NoContent();
    }

    [HttpPut("{id}/TogglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.ToggleStatusAsync(id, cancellationToken);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }
}
