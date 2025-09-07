using Microsoft.AspNetCore.Identity;
using SurveyBasket.API.Contracts.Users;

namespace SurveyBasket.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<TResult<UserProfileResponse>> UserProfile(string id)
        {
           var userProfile= await _userManager.Users.Where(x=>x.Id==id).ProjectToType<UserProfileResponse>().SingleAsync();

            return Result.Succes(userProfile);
        }

        public async Task<Result> UpdateProfile(string id, UpdateProfile profile)
        {
            await _userManager.Users.Where(x=>x.Id==id).ExecuteUpdateAsync(updates => updates
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
