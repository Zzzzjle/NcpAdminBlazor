using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate
{
    public partial record RoleMenuPermissionId : IGuidStronglyTypedId;

    public class RoleMenuPermission : Entity<RoleMenuPermissionId>
    {
        protected RoleMenuPermission()
        {
        }

        public RoleId RoleId { get; private set; } = null!;
        public MenuId MenuId { get; private set; } = null!;
        public bool IsDisabled { get; private set; } = false;
        public string PermissionCode { get; private set; } = string.Empty;

        public RoleMenuPermission(MenuId menuId, string permissionCode)
        {
            MenuId = menuId;
            PermissionCode = permissionCode;
        }
    }
}