namespace NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate
{
    public partial record RolePermissionId : IInt64StronglyTypedId;

    public class RolePermission : Entity<RolePermissionId>
    {
        protected RolePermission()
        {
        }

        public RoleId RoleId { get; internal set; } = null!;
        public string PermissionCode { get; private set; } = string.Empty;

        public RolePermission(string permissionCode)
        {
            PermissionCode = permissionCode;
        }
    }
}