using SurveyBasket.API.Contracts.Users;
using SurveyBasket.API.Controllers;

namespace SurveyBasket.API.Services
{
    public interface IUserService
    {
        public Task<TResult<UserProfileResponse>> UserProfile(string id);

        public Task<Result> UpdateProfile(string id, UpdateProfile profile);

        Task<Result> ChangePassword(string userId, ChangePasswordRequest changePasswordRequest);

        public Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        public Task<TResult<UserResponse>> GetUserAsync(string Id, CancellationToken cancellationToken = default);

        public Task<TResult<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

        public Task<Result> UpdateAsync(string Id, UpdateUserRequest request, CancellationToken cancellationToken = default);



        public Task<Result> ToggleStatus(string Id, CancellationToken cancellationToken);

        public Task<Result> Unlock(string id, CancellationToken cancellationToken = default);




    }
}
