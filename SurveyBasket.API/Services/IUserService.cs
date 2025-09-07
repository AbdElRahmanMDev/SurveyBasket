using SurveyBasket.API.Contracts.Users;

namespace SurveyBasket.API.Services
{
    public interface IUserService
    {
        public Task<TResult<UserProfileResponse>> UserProfile(string id);

        public Task<Result> UpdateProfile(string id, UpdateProfile profile);

        Task<Result> ChangePassword(string userId, ChangePasswordRequest changePasswordRequest);


    }
}
