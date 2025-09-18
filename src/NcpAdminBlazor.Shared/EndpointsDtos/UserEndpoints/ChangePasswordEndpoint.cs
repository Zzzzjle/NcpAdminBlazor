using System.ComponentModel.DataAnnotations;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Shared.EndpointsDtos.UserEndpoints;

/// <summary>
/// 修改密码请求模型
/// </summary>
public class ChangePasswordRequest
{
    [Required(ErrorMessage = "用户ID不能为空")]
    public required ApplicationUserId UserId { get; set; }

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
