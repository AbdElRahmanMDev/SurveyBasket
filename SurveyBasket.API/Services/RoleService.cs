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
            var role = await _context.Roles
              .Where(x => x.Id == request.RoleId)
              .SingleOrDefaultAsync(cancellationToken);

            if (role is null)
                return Result.Failure(RoleErrors.RoleNotFound);


            var Allpermissons = Permissions.GetAllPermissons();


            if (request.Permissions.Except(Allpermissons).Any())
                return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermission);

            role.Name = request.Name;

            var roleClaims = await _context.RoleClaims.Where(x => x.RoleId == request.RoleId).ToListAsync(cancellationToken);

            var deletedroleClaims = roleClaims
                .Where(x => request.Permissions.Contains(x.ClaimValue!) == false)
                .ToList();
            _context.RoleClaims.RemoveRange(deletedroleClaims);
            await _context.SaveChangesAsync(cancellationToken);



            var result = await _roleManager.UpdateAsync(role);
            //Different Scenarios In Update 
            //New Data that isn't In Db in that case you need to delete data in Db
            //Data 


            if (result.Succeeded)
            {
                var existingClaims = await _context.RoleClaims
                    .Where(x => x.RoleId == role.Id)
                    .Select(x => x.ClaimValue!)
                    .ToListAsync(cancellationToken);

                //Existing permission In Db
                //newData that doesn't exist In Db
                var newPermissionData = request.Permissions.Except(existingClaims).ToList();


                var permissons = newPermissionData.
                    Select(x => new IdentityRoleClaim<string>()
                    {
                        ClaimType = Permissions.Type,
                        ClaimValue = x,
                        RoleId = role.Id
                    });

                await _context.RoleClaims.AddRangeAsync(permissons);
                await _context.SaveChangesAsync();

                var response = new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, request.Permissions);
                return Result.Succes(response);
            }

            var error = result.Errors.First();

            return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));



        }


    }
}
