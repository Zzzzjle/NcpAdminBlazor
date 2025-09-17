using Refit;
using NcpAdminBlazor.Client.Client.Models;
using NcpAdminBlazor.Shared.Models;
using NetCorePal.Extensions.Dto;

namespace NcpAdminBlazor.Client.Client.Apis;

public interface IUserApi
{
    [Post("/api/user/login")]
    Task<ApiResponse<ResponseData<LoginResponse>>> LoginAsync([Body] LoginRequest loginRequest);
    
    [Post("/api/user/register")]
    Task<ApiResponse<ResponseData<RegisterResponse>>> RegisterAsync([Body] RegisterRequest registerRequest);
    
    [Get("/api/user/{userId}")]
    Task<ApiResponse<ResponseData<UserInfoResponse>>> GetUserInfoAsync(string userId, [Header("Authorization")] string authorization);
    
    [Put("/api/user/password")]
    Task<ApiResponse<ResponseData<object>>> ChangePasswordAsync([Body] ChangePasswordRequest changePasswordRequest, [Header("Authorization")] string authorization);
    
    [Get("/api/user/auth")]
    Task<ApiResponse<ResponseData<bool>>> CheckAuthAsync([Header("Authorization")] string authorization);
}
