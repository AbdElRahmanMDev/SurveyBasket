using SurveyBasket.API.Abstraction.Consts;
using SurveyBasket.API.Authentication.Filters;
using SurveyBasket.API.Contracts.Users;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUsers")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> getAllUsers(CancellationToken cancellationToken)
        {
            var result = await _userService.GetAllAsync(cancellationToken);

            return Ok(result);
        }


        [HttpGet("{id}")]

        public async Task<IActionResult> getUserById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            var result = await _userService.GetUserAsync(id, cancellationToken);
            if (result.IsFailure)
                return result.ToProblem();
            return Ok(result.Value);
        }


        [HttpPost("AddUser")]

        public async Task<IActionResult> AddNewUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _userService.AddAsync(request, cancellationToken);
            if (result.IsFailure)
                return result.ToProblem();
            return Ok(result.Value);
        }



        [HttpPut("UpdateUser/{Id}")]

        public async Task<IActionResult> UpdateUser([FromRoute] string Id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateAsync(Id, request, cancellationToken);

            if (result.IsSuccess)
                return NoContent();

            return result.ToProblem();

        }


        [HttpPut("{id}/toggle-status")]

        public async Task<IActionResult> ToggleStatus([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _userService.ToggleStatus(id, cancellationToken);
            if (result.IsSuccess)
                return NoContent();
            return result.ToProblem();
        }


        [HttpPut("{id}/unlock")]

        public async Task<IActionResult> UnlcckUser([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _userService.Unlock(id, cancellationToken);
            if (result.IsSuccess)
                return NoContent();
            return result.ToProblem();
        }
    }
}
