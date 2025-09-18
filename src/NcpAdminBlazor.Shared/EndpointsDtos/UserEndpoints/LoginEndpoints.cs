using System.ComponentModel.DataAnnotations;

namespace NcpAdminBlazor.Shared.EndpointsDtos.UserEndpoints;

/// <summary>
/// 用户登录请求模型
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "登录名不能为空")]
    [Display(Name = "用户名/邮箱/手机号")]
    public string LoginName { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [DataType(DataType.Password)]
    [Display(Name = "密码")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 用户登录响应模型
/// </summary>
public class LoginResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public string Token { get; set; } = string.Empty;
}