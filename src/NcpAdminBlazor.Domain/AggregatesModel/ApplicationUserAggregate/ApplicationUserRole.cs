using RoleId = NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate.RoleId;

namespace NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate
{
    public partial record ApplicationUserRoleId : IInt64StronglyTypedId;
    public class ApplicationUserRole : Entity<ApplicationUserRoleId>
    {
        protected ApplicationUserRole() { }

        public ApplicationUserId ApplicationUserId { get; private set; } = null!;
        public RoleId RoleId { get; private set; } = null!;
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
