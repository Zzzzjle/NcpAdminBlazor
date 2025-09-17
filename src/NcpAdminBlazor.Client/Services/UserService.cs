using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using NcpAdminBlazor.Client.Client.Apis;
using NcpAdminBlazor.Client.Client.Models;
using NcpAdminBlazor.Client.Client.Providers;
using NcpAdminBlazor.Shared.Models;
using System.Text.Json;

namespace NcpAdminBlazor.Client.Client.Services
{
    public interface IUserService
    {
        Task<LoginResultExtended> LoginAsync(LoginRequest loginRequest);
        Task<RegisterResultExtended> RegisterAsync(RegisterRequest registerRequest);
        Task<UserInfoResponse> GetUserInfoAsync(string userId);
        Task<bool> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
    }

    public class UserService : IUserService
    {
        private readonly IUserApi _userApi;
        private readonly ILocalStorageService _localStorage;
        private readonly TokenAuthenticationStateProvider _authStateProvider;

        public UserService(IUserApi userApi, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _userApi = userApi;
            _localStorage = localStorage;
            _authStateProvider = (TokenAuthenticationStateProvider)authStateProvider;
        }

        public async Task<LoginResultExtended> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var response = await _userApi.LoginAsync(loginRequest);
                if (response is { IsSuccessStatusCode: true, Content: not null })
                {
                    var loginResult = response.Content.Data;
                    if (loginResult != null)
                    {
                        var extendedResult = new LoginResultExtended
                        {
                            UserId = loginResult.UserId,
                            Name = loginResult.Name,
                            Email = loginResult.Email,
                            RealName = loginResult.RealName,
                            Roles = loginResult.Roles,
                            Permissions = loginResult.Permissions,
                            Token = loginResult.Token,
                            Successful = true
                        };
                        
                        // 保存token
                        await _localStorage.SetItemAsync("token", loginResult.Token);
                        await _localStorage.SetItemAsync("user", extendedResult);
                        
                        // 通知认证状态更改
                        await _authStateProvider.Login(loginResult.Token);
                        
                        return extendedResult;
                    }
                }
                
                var errorContent = response.Error?.Content;
                return new LoginResultExtended
                {
                    Successful = false,
                    Error = errorContent ?? "登录失败"
                };
            }
            catch (Exception ex)
            {
                return new LoginResultExtended
                {
                    Successful = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<RegisterResultExtended> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                var response = await _userApi.RegisterAsync(registerRequest);
                if (response.IsSuccessStatusCode && response.Content?.Data != null)
                {
                    return new RegisterResultExtended
                    {
                        UserId = response.Content.Data.UserId,
                        Successful = true
                    };
                }
                else
                {
                    var errorContent = response.Error?.Content;
                    return new RegisterResultExtended
                    {
                        Successful = false,
                        Errors = new[] { errorContent ?? "注册失败" }
                    };
                }
            }
            catch (Exception ex)
            {
                return new RegisterResultExtended
                {
                    Successful = false,
                    Errors = new[] { ex.Message }
                };
            }
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string userId)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("未登录");

            var response = await _userApi.GetUserInfoAsync(userId, $"Bearer {token}");
            if (response.IsSuccessStatusCode && response.Content?.Data != null)
            {
                return response.Content.Data;
            }
            
            throw new Exception(response.Error?.Content ?? "获取用户信息失败");
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                var response = await _userApi.ChangePasswordAsync(changePasswordRequest, $"Bearer {token}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("token");
            await _localStorage.RemoveItemAsync("user");
            await _authStateProvider.Logout();
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                var response = await _userApi.CheckAuthAsync($"Bearer {token}");
                return response.IsSuccessStatusCode && response.Content?.Data == true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("token") ?? string.Empty;
        }
    }
}