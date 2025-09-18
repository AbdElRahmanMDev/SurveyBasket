using SurveyBasket.API.Abstraction.Consts;
using SurveyBasket.API.Authentication.Filters;
using SurveyBasket.API.Contracts.Roles;

namespace SurveyBasket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    [HttpGet]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled, CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllRolesAsync(includeDisabled, cancellationToken);
        return Ok(roles);
    }

    [HttpGet("GetById/{roleId}")]
    [HasPermission(Permissions.GetRoles)]

    public async Task<IActionResult> Get([FromRoute] string roleId, CancellationToken cancellationToken)
    {
        var result = await _roleService.GetRoleByIdAsync(roleId, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);

    }

    [HttpPost("AddNewRole")]
    [HasPermission(Permissions.AddRoles)]
    public async Task<IActionResult> AddRole([FromBody] RoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _roleService.AddNewRoleWithPermssion(request, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }


    [HttpPut("UpdateRole")]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRole request, CancellationToken cancellationToken)
    {
        var result = await _roleService.UpdateRoleAndPermisson(request, cancellationToken);

        if (!result.IsSuccess)
            return result.ToProblem();

        return NoContent();
    }

    [HttpPut("ToggleStatus/{Id}")]

    public async Task<IActionResult> ToggleStatus([FromRoute] string Id, CancellationToken cancellationToken)
    {
        var result = await _roleService.ToggleStatus(Id, cancellationToken);

        if (!result.IsSuccess)
            return result.ToProblem();

        return NoContent();
    }


}
