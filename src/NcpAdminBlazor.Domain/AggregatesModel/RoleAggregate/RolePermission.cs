namespace NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate
{
    public class RolePermission
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