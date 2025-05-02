using MapsterMapper;

namespace SurveyBasket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController : ControllerBase
{
    private readonly IPollService _pollService;
    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }

    
    [HttpGet]
    public IActionResult GetAll()
    {
        var polls=_pollService.GetAllPolls();
        var responses=polls.Adapt<IEnumerable<PollResponse>>();
        return Ok(responses);
    }


    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
       var poll=_pollService.GetpollById(id);

        if (poll is null)
            return NotFound();

        var response = poll.Adapt<PollResponse>();
        return Ok(response);
    }

    [HttpPost("")]
    public IActionResult Add([FromBody]CreatePollRequest pollRequest)
    {
        var poll = pollRequest.Adapt<Poll>();
        var newPoll = _pollService.Add(poll);
       return CreatedAtAction(nameof(GetById), new {id= newPoll.Id},newPoll);
         
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromRoute]int id,[FromBody]CreatePollRequest pollmodel) {

        var poll = pollmodel.Adapt<Poll>();
        var isUpdated =_pollService.Update(id,poll);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute]int id) {
       var isDeleted= _pollService.Delete(id);

        if (!isDeleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost("test")]
    public IActionResult Test([FromBody]Student student)
    {
        return Ok("Value Accepted");
    }

}
