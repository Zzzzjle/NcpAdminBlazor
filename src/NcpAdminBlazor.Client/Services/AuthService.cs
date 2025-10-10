using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace NcpAdminBlazor.Client.Services;

/// <summary>
/// 认证服务，管理认证状态并提供优化的权限检查
/// </summary>
public class AuthService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private AuthenticationState? _cachedAuthState;
    private bool _isInitialized;

    public event Action? OnAuthenticationStateChanged;

    public AuthService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
        
        // 订阅认证状态变化
        _authenticationStateProvider.AuthenticationStateChanged += HandleAuthenticationStateChanged;
    }

    /// <summary>
    /// 获取当前认证状态
    /// </summary>
    public async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_cachedAuthState == null || !_isInitialized)
        {
            _cachedAuthState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            _isInitialized = true;
        }
        
        return _cachedAuthState;
    }

    /// <summary>
    /// 检查用户是否已认证
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await GetAuthenticationStateAsync();
        return authState.User?.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// 检查用户是否在指定角色中
    /// </summary>
    public async Task<bool> IsInRoleAsync(string role)
    {
        var authState = await GetAuthenticationStateAsync();
        return authState.User?.IsInRole(role) ?? false;
    }

    /// <summary>
    /// 检查用户是否拥有指定权限
    /// </summary>
    public async Task<bool> HasPermissionAsync(string permission)
    {
        var authState = await GetAuthenticationStateAsync();
        return authState.User?.HasClaim("permission", permission) ?? false;
    }

    /// <summary>
    /// 获取当前用户
    /// </summary>
    public async Task<ClaimsPrincipal?> GetUserAsync()
    {
        var authState = await GetAuthenticationStateAsync();
        return authState.User;
    }

    /// <summary>
    /// 手动触发认证状态变更通知（用于登录/登出后）
    /// </summary>
    public void NotifyAuthenticationStateChanged()
    {
        _cachedAuthState = null;
        _isInitialized = false;
        OnAuthenticationStateChanged?.Invoke();
    }

    private async void HandleAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        _cachedAuthState = await task;
        _isInitialized = true;
        OnAuthenticationStateChanged?.Invoke();
    }
}
