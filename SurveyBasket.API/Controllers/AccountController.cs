using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg.Sig;
using SurveyBasket.API.Contracts.Users;
using SurveyBasket.API.Extensions;

namespace SurveyBasket.API.Controllers
{
    [Route("me")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Info()
        {
            var result = await _userService.UserProfile(User.GetUserId()!);

            return Ok(result.Value);
        }

        [HttpPut("info")]

        public async Task<IActionResult> Update([FromBody] UpdateProfile profile)
        {
            await _userService.UpdateProfile(User.GetUserId()!, profile);

            return NoContent(); 
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _userService.ChangePassword(User.GetUserId()!, request);

            if (result.IsSuccess)
                return Ok(new { message = "Password changed successfully" });

            return BadRequest(new { errors = result.Error });
        }
    }
}
