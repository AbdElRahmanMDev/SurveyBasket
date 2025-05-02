using Microsoft.AspNetCore.Http;
using SurveyBasket.API.Services;


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
        return Ok(polls);
    }


    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
       var poll=_pollService.GetpollById(id);
       return poll is null ? NotFound() : Ok(poll);
    }

    [HttpPost("")]
    public IActionResult Add(Poll poll)
    {
        var newPoll = _pollService.Add(poll);
       return CreatedAtAction(nameof(GetById), new {id= newPoll.Id},newPoll);
         
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id,Poll poll) { 
        
        var isUpdated=_pollService.Update(id,poll);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    
    }

    [HttpDelete("{id}")]

    public IActionResult Delete(int id) { 
    
       var isDeleted= _pollService.Delete(id);

        if (!isDeleted)
            return NotFound();

        return NoContent();

    
    }

}
