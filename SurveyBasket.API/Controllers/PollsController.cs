using Asp.Versioning;
using SurveyBasket.API.Abstraction.Consts;
using SurveyBasket.API.Authentication.Filters;

namespace SurveyBasket.API.Controllers;

[ApiVersion(1)]
[ApiVersion(2)]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
//[Authorize]

public class PollsController : ControllerBase
{
    private readonly IPollService _pollService;
    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }




    [HttpGet]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetAllPollsAsync(cancellationToken);
        return Ok(polls);
    }

    [HttpGet("GetCurrent")]
    //[Authorize(Roles = DefaultRoles.MemberRoleName)]
    [MapToApiVersion(1)]

    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetCurrentAsyncV1(cancellationToken);
        return Ok(polls);
    }


    [HttpGet("GetCurrent")]
    [MapToApiVersion(2)]
    //[Authorize(Roles = DefaultRoles.MemberRoleName)]
    public async Task<IActionResult> GetCurrentV2(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetCurrentAsyncV2(cancellationToken);
        return Ok(polls);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetpollByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }

    [HttpPost("")]
    [HasPermission(Permissions.AddPolls)]
    public async Task<IActionResult> Add([FromBody] PollRequest pollRequest, CancellationToken cancellationToken)
    {
        var newPoll = await _pollService.AddAsync(pollRequest, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = newPoll.Value.Id }, newPoll.Value);

    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollmodel, CancellationToken cancellationToken)
    {

        var poll = pollmodel.Adapt<Poll>();
        var result = await _pollService.UpdateAysnc(id, poll, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();


    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{id}/TogglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.ToggleStatusAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
