using System.ComponentModel.DataAnnotations;

namespace NcpAdminBlazor.Shared.Models;

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
public class RegisterResponse
{
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// 获取用户信息请求模型
/// </summary>
public class GetUserInfoRequest
{
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// 用户信息响应模型
/// </summary>
public class UserInfoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<UserRoleDto> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}

/// <summary>
/// 用户角色DTO
/// </summary>
public class UserRoleDto
{
    public string RoleName { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
}

/// <summary>
/// 修改密码请求模型
/// </summary>
public class ChangePasswordRequest
{
    [Required(ErrorMessage = "用户ID不能为空")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "旧密码不能为空")]
    [DataType(DataType.Password)]
    [Display(Name = "旧密码")]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(50, ErrorMessage = "新密码长度必须在 {2} 到 {1} 个字符之间。", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "新密码")]
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// 获取用户列表请求模型
/// </summary>
public class GetUserListRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// 用户列表项DTO
/// </summary>
public class UserListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<string> RoleNames { get; set; } = new();
}