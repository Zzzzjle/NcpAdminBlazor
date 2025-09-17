using RoleId = NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate.RoleId;

namespace NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate
{
    public partial record ApplicationUserPermissionId : IInt64StronglyTypedId;

    public class ApplicationUserPermission : Entity<ApplicationUserPermissionId>
    {
        protected ApplicationUserPermission()
        {
        }

        public ApplicationUserId ApplicationUserId { get; private set; } = null!;
        public string PermissionCode { get; private set; } = string.Empty;
        public ICollection<RoleId> SourceRoleIds { get; } = [];

        public ApplicationUserPermission(string permissionCode, RoleId? sourceRoleId = null)
        {
            PermissionCode = permissionCode;
            if (sourceRoleId is not null)
            {
                SourceRoleIds.Add(sourceRoleId);
            }
        }

        public void AddSourceRoleId(RoleId roleId)
        {
            if (SourceRoleIds.Contains(roleId)) return;
            SourceRoleIds.Add(roleId);
        }
    }
}