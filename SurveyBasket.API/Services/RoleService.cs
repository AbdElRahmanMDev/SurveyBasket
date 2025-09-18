using Microsoft.AspNetCore.Identity;
using SurveyBasket.API.Abstraction.Consts;
using SurveyBasket.API.Contracts.Roles;
using SurveyBasket.API.Persistence;

namespace SurveyBasket.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleService(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }
        public async Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default)
        {
            return await _context.Roles.Where(x => !x.IsDefault && (!x.IsDeleted || includeDisabled == true)).
                 Select(x => new RoleResponse(x.Id, x.Name, x.IsDeleted)).
                 ToListAsync(cancellationToken);

        }

        public async Task<TResult<RoleDetailResponse>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {

            var role = await _context.Roles
                .Where(x => x.Id == roleId && !x.IsDefault)
                .SingleOrDefaultAsync(cancellationToken);

            var permissons = await _context.RoleClaims.Where(x => x.RoleId == roleId)
                .Select(x => x.ClaimValue!)
                .ToListAsync(cancellationToken);
            if (role is null)
                return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

            var response = new RoleDetailResponse(roleId, role.Name!, role.IsDeleted, permissons);
            return Result.Succes(response);

        }

        public async Task<TResult<RoleDetailResponse>> AddNewRoleWithPermssion(RoleRequest request, CancellationToken cancellationToken = default)
        {
            var role = await _context.Roles
              .Where(x => x.Name == request.Name)
              .SingleOrDefaultAsync(cancellationToken);

            if (role is not null)
                return Result.Failure<RoleDetailResponse>(RoleErrors.RoleAlreadyExists);


            var Allpermissons = Permissions.GetAllPermissons();

            var newPermissons = request.Permissions.Except(Allpermissons).ToList();

            if (request.Permissions.Except(Allpermissons).Any())
                return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermission);

            var newRole = new ApplicationRole()
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsDefault = false,
                IsDeleted = false
            };

            var result = await _roleManager.CreateAsync(newRole);

            if (result.Succeeded)
            {
                var permissons = request.Permissions.
                    Select(x => new IdentityRoleClaim<string>()
                    {
                        ClaimType = Permissions.Type,
                        ClaimValue = x,
                        RoleId = newRole.Id
                    });

                await _context.RoleClaims.AddRangeAsync(permissons);
                await _context.SaveChangesAsync();

                var response = new RoleDetailResponse(newRole.Id, newRole.Name, newRole.IsDeleted, request.Permissions);
                return Result.Succes(response);
            }

            var error = result.Errors.First();

            return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));



        }
        public async Task<Result> UpdateRoleAndPermisson(UpdateRole request, CancellationToken cancellationToken = default)
        {

            var role = await _context.Roles.SingleOrDefaultAsync(x => x.Id == request.RoleId);
            if (role is null)
                return Result.Failure(RoleErrors.RoleNotFound);

            var allPermissionsInSystem = Permissions.GetAllPermissons();
            var IsPermissionExist = request.Permissions;
            if (IsPermissionExist.Except(allPermissionsInSystem).Any())
                return Result.Failure(RoleErrors.InvalidPermission);

            role.Name = request.Name;

            var result = await _roleManager.SetRoleNameAsync(role, request.Name);

            if (result.Succeeded)
            {
                var currentPermissions = await _context.RoleClaims.Where(x => x.RoleId == request.RoleId)
               .Select(x => x.ClaimValue!)
               .ToListAsync(cancellationToken);

                var requestPermissions = request.Permissions;

                var newAddedPermissions = request.Permissions.Except(currentPermissions);

                var removedPermissions = currentPermissions.Except(requestPermissions);

                if (newAddedPermissions.Any())
                {
                    var newPermissons = newAddedPermissions.
                        Select(x => new IdentityRoleClaim<string>()
                        {
                            ClaimType = Permissions.Type,
                            ClaimValue = x,
                            RoleId = request.RoleId
                        });
                    await _context.RoleClaims.AddRangeAsync(newPermissons);
                }


                await _context.RoleClaims.Where(x => removedPermissions.Contains(x.ClaimValue)).ExecuteDeleteAsync(cancellationToken);

                await _context.SaveChangesAsync();

                return Result.Succes("Role Update Successfully");
            }

            var error = result.Errors.FirstOrDefault();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status404NotFound));
        }




        public async Task<Result> ToggleStatus(string RoleId, CancellationToken cancellationToken = default)
        {
            var role = await _context.Roles.SingleOrDefaultAsync(x => x.Id == RoleId);
            if (role is null)
                return Result.Failure(RoleErrors.RoleNotFound);

            role.IsDeleted = !role.IsDeleted;

            await _roleManager.UpdateAsync(role);

            return Result.Succes();

        }
    }
}
