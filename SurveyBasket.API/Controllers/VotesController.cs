using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.API.Abstraction.Consts;
using SurveyBasket.API.Contracts.Votes;
using SurveyBasket.API.Extensions;
using System.Security.Claims;

namespace SurveyBasket.API.Controllers
{
    [Route("api/polls/{pollId}/vote")]
    [ApiController]
    [Authorize(Roles =DefaultRoles.MemberRoleName)]
    public class VotesController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly IVoteService _voteService;
        public VotesController(IQuestionService questionService, IVoteService voteService)
        {
            _questionService = questionService;
            _voteService = voteService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Get([FromRoute] int pollId,CancellationToken cancellationToken)
        {
            var userId=User.GetUserId();
            var result = await _questionService.GetAvailableAsync(pollId, userId!, cancellationToken);
          

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();


        }

        [HttpPost("")]

        public async Task<IActionResult> AddVote([FromRoute] int pollId,[FromBody]VoteRequest voteRequest,CancellationToken cancellationToken)
        {
            var result=await _voteService.AddVoteAsync(pollId,User.GetUserId()!,voteRequest,cancellationToken);

            if (result.IsSuccess)
                return Created();

            return result.IsSuccess ? Created() : result.ToProblem();


        }


    }
}
