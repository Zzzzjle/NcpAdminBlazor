using System.ComponentModel.DataAnnotations;

namespace NcpAdminBlazor.Client.Client.Models
{
    public class LoginModel
    {
        [Required] [EmailAddress] public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")] public bool RememberMe { get; set; }
    }
}