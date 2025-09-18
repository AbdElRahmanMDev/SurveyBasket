using Microsoft.AspNetCore.Identity;
using SurveyBasket.API.Abstraction.Consts;
using SurveyBasket.API.Contracts.Roles;
using SurveyBasket.API.Contracts.Users;
using SurveyBasket.API.Controllers;
using SurveyBasket.API.Persistence;

namespace SurveyBasket.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IRoleService _roleService;
        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IRoleService roleService)
        {
            _userManager = userManager;
            _context = context;
            _roleService = roleService;
        }


        public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
               await (from u in _context.Users
                      join ur in _context.UserRoles
                      on u.Id equals ur.UserId
                      join r in _context.Roles
                      on ur.RoleId equals r.Id into roles
                      where !roles.Any(x => x.Name == DefaultRoles.MemberRoleName)
                      select new
                      {
                          u.Id,
                          u.FirstName,
                          u.LastName,
                          u.Email,
                          u.IsDisabled,
                          Roles = roles.Select(x => x.Name!).ToList()
                      }
                       )
                       .GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled })
                       .Select(u => new UserResponse
                       (
                           u.Key.Id,
                           u.Key.FirstName,
                           u.Key.LastName,
                           u.Key.Email,
                           u.Key.IsDisabled,
                           u.SelectMany(x => x.Roles)
                       ))
                      .ToListAsync(cancellationToken);



        public async Task<TResult<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {

            var user = await _context.Users.Where(x => x.Email == request.Email).SingleOrDefaultAsync(cancellationToken);
            if (user is not null)
                return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

            var allPermissions = Permissions.GetAllPermissons();

            IEnumerable<RoleResponse> allRoles = await _roleService.GetAllRolesAsync();

            if (request.Roles.Except(allRoles.Select(x => x.Name)).Any())
                return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

            var newAddUser = new ApplicationUser()
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,

            };
            newAddUser.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(newAddUser, request.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }
            await _userManager.AddToRolesAsync(newAddUser, request.Roles);

            var response = new UserResponse(newAddUser.Id, newAddUser.FirstName, newAddUser.LastName, newAddUser.Email, newAddUser.IsDisabled, request.Roles);
            return Result.Succes(response);
        }

        public async Task<Result> UpdateAsync(string Id, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.Where(x => x.Email == request.Email && x.Id != Id).SingleOrDefaultAsync(cancellationToken);
            if (user is not null)
                return Result.Failure(UserErrors.DuplicatedEmail);

            user = await _context.Users.Where(x => x.Id == Id).SingleOrDefaultAsync(cancellationToken);
            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            var allPermissions = Permissions.GetAllPermissons();

            IEnumerable<RoleResponse> allRoles = await _roleService.GetAllRolesAsync();

            if (request.Roles.Except(allRoles.Select(x => x.Name)).Any())
                return Result.Failure(UserErrors.InvalidRoles);


            user = request.Adapt(user);

            user!.NormalizedEmail = request.Email.ToUpper();

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _context.UserRoles.Where(x => x.UserId == Id).ExecuteDeleteAsync();

                await _userManager.AddToRolesAsync(user, request.Roles);

                return Result.Succes();

            }

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }


        public async Task<Result> Unlock(string id, CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }
            return Result.Succes();
        }

        public async Task<Result> ToggleStatus(string Id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Where(x => x.Id == Id).SingleOrDefaultAsync(cancellationToken);
            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);


            user.IsDisabled = !user.IsDisabled;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            return Result.Succes();
        }

        public async Task<TResult<UserResponse>> GetUserAsync(string Id, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users.Where(x => x.Id == Id).SingleOrDefaultAsync(cancellationToken);
            if (user is null)
                return Result.Failure<UserResponse>(UserErrors.UserNotFound);

            var userRoles = await _userManager.GetRolesAsync(user);
            var response = new UserResponse(user.Id, user.FirstName, user.LastName, user.Email, user.IsDisabled, userRoles);
            return Result.Succes(response);

        }

        public async Task<TResult<UserProfileResponse>> UserProfile(string id)
        {
            var userProfile = await _userManager.Users.Where(x => x.Id == id).ProjectToType<UserProfileResponse>().SingleAsync();

            return Result.Succes(userProfile);
        }

        public async Task<Result> UpdateProfile(string id, UpdateProfile profile)
        {
            await _userManager.Users.Where(x => x.Id == id).ExecuteUpdateAsync(updates => updates
                    .SetProperty(u => u.FirstName, u => profile.FirstName)
                    .SetProperty(u => u.LastName, u => profile.LastName)
                );

            return Result.Succes();
        }


        public async Task<Result> ChangePassword(string userId, ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userManager.Users.Where(x => x.Id == userId).SingleAsync();
            var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.currentPassword, changePasswordRequest.newPassword);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }
            return Result.Succes();
        }

    }
}
