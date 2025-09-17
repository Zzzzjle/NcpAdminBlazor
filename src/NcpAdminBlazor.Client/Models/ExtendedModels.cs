using NcpAdminBlazor.Shared.Models;

namespace NcpAdminBlazor.Client.Client.Models;

/// <summary>
/// 登录结果扩展模型（兼容旧代码）
/// </summary>
public class LoginResultExtended : LoginResponse
{
    public bool Successful { get; set; }
    public string Error { get; set; } = string.Empty;
}

/// <summary>
/// 注册结果扩展模型（兼容旧代码）
/// </summary>
public class RegisterResultExtended : RegisterResponse
{
    public bool Successful { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();
}

/// <summary>
/// 用户详情扩展模型（兼容旧代码）
/// </summary>
public class UserDetailsExtended : UserInfoResponse
{
    // 如果需要额外属性可以在这里添加
}

/// <summary>
/// 修改密码扩展模型（兼容旧代码）
/// </summary>
public class ChangePasswordExtended : ChangePasswordRequest
{
    // 如果需要额外属性可以在这里添加
}