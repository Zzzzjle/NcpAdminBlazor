using System.ComponentModel.DataAnnotations;

namespace NcpAdminBlazor.Client.Client.Models
{
    public class LoginModel
    {
        [Required] 
        [Display(Name = "用户名/邮箱/手机号")]
        public string LoginName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "记住我")] 
        public bool RememberMe { get; set; }
    }
}