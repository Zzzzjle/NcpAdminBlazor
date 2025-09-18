using System.ComponentModel.DataAnnotations;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Shared.EndpointsDtos.UserEndpoints;

/// <summary>
/// 用户注册请求模型
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "用户名不能为空")]
    [Display(Name = "用户名")]
    [StringLength(50, ErrorMessage = "用户名不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [Display(Name = "邮箱")]
    [StringLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(50, ErrorMessage = "密码长度必须在 {2} 到 {1} 个字符之间。", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "密码")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "手机号不能为空")]
    [Display(Name = "手机号")]
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "真实姓名不能为空")]
    [Display(Name = "真实姓名")]
    [StringLength(50, ErrorMessage = "真实姓名不能超过50个字符")]
    public string RealName { get; set; } = string.Empty;
}

/// <summary>
/// 用户注册响应模型
/// </summary>
public record RegisterResponse(ApplicationUserId UserId);