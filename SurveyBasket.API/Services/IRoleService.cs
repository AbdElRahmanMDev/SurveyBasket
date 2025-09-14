using SurveyBasket.API.Contracts.Roles;

namespace SurveyBasket.API.Services
{
    public interface IRoleService
    {

        public Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default);
        public Task<TResult<RoleDetailResponse>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);

        public Task<TResult<RoleDetailResponse>> AddNewRoleWithPermssion(RoleRequest request, CancellationToken cancellationToken = default);


    }
}
