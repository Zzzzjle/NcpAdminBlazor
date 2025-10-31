using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

public partial record UserMenuPermissionId : IGuidStronglyTypedId;

public class UserMenuPermission : Entity<UserMenuPermissionId>
{
    protected UserMenuPermission()
    {
    }

    public ApplicationUserId UserId { get; private set; } = null!;
    public MenuId MenuId { get; private set; } = null!;
    public RoleId SourceRoleId { get; private set; } = null!;
    public string PermissionCode { get; private set; } = string.Empty;


    public UserMenuPermission(MenuId menuId, RoleId sourceRoleId, string permissionCode)
    {
        MenuId = menuId;
        SourceRoleId = sourceRoleId;
        PermissionCode = permissionCode;
    }
}