using RoleId = NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate.RoleId;

namespace NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate
{
    public class ApplicationUserRole
    {
        protected ApplicationUserRole() { }

        public ApplicationUserId ApplicationUserId { get; private set; } = default!;
        public RoleId RoleId { get; private set; } = default!;
        public string RoleName { get; private set; } = string.Empty;

        public ApplicationUserRole(RoleId roleId, string roleName)
        {
            RoleId = roleId;
            RoleName = roleName;
        }

        public void UpdateRoleInfo(string roleName)
        {
            RoleName = roleName;
        }
    }
}
