using Refit;
using NcpAdminBlazor.Shared.EndpointsDtos.UserEndpoints;
using NetCorePal.Extensions.Dto;

namespace NcpAdminBlazor.Client.Client.Apis;

public interface IUserApi
{
    [Post("/api/user/login")]
    Task<ResponseData<LoginResponse>> LoginAsync([Body] LoginRequest loginRequest);
    
    [Post("/api/user/register")]
    Task<ResponseData<RegisterResponse>> RegisterAsync([Body] RegisterRequest registerRequest);
    
    [Get("/api/user/{userId}")]
    Task<ResponseData<UserInfoResponse>> GetUserInfoAsync(string userId, [Header("Authorization")] string authorization);
    
    [Put("/api/user/password")]
    Task<ResponseData<object>> ChangePasswordAsync([Body] ChangePasswordRequest changePasswordRequest, [Header("Authorization")] string authorization);
    
    [Get("/api/user/auth")]
    Task<ResponseData<bool>> CheckAuthAsync([Header("Authorization")] string authorization);
}
