namespace NcpAdminBlazor.Client.Models
{
    public class UserDetails
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string RealName { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<UserRole> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }

    public class UserRole
    {
        public string RoleName { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
    }

    public class ChangePasswordModel
    {
        public string UserId { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}