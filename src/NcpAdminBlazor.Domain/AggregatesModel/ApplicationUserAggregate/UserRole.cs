using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate
{
    public partial record ApplicationUserRoleId : IGuidStronglyTypedId;

    public class UserRole : Entity<ApplicationUserRoleId>
    {
        protected UserRole()
        {
        }

        public ApplicationUserId UserId { get; private set; } = null!;
        public RoleId RoleId { get; private set; } = null!;
        public string RoleName { get; private set; } = string.Empty;
        public bool IsDisabled { get; private set; } = false;

        public UserRole(RoleId roleId, string roleName)
        {
            RoleId = roleId;
            RoleName = roleName;
        }

        public void UpdateUserRoleInfo(string roleName, bool isDisabled)
        {
            RoleName = roleName;
            IsDisabled = isDisabled;
        }
    }
}