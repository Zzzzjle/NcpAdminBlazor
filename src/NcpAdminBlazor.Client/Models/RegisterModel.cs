using System.ComponentModel.DataAnnotations;

namespace NcpAdminBlazor.Client.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "用户名")]
        [StringLength(50, ErrorMessage = "用户名不能超过50个字符")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        [Display(Name = "邮箱")]
        [StringLength(100, ErrorMessage = "邮箱不能超过100个字符")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "密码长度必须在 {2} 到 {1} 个字符之间。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "手机号")]
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "真实姓名")]
        [StringLength(50, ErrorMessage = "真实姓名不能超过50个字符")]
        public string RealName { get; set; } = string.Empty;
    }
}
