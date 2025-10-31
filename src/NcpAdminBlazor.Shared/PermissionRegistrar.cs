using FluentPermissions.Core.Abstractions;
using FluentPermissions.Core.Builder;
using NcpAdminBlazor.Shared.Auth;

namespace NcpAdminBlazor.Shared;

// ReSharper disable once UnusedType.Global
public class PermissionRegistrar : IPermissionRegistrar
{
    #region

    private const string View = "View";
    private const string Create = "Create";
    private const string Edit = "Edit";
    private const string Delete = "Delete";

    #endregion

    public void Register(PermissionBuilder builder)
    {
        builder.DefineGroup("System", system =>
        {
            system.DefineGroup("Users", users =>
            {
                users.AddPermission(View);
                users.AddPermission(Create);
                users.AddPermission(Edit);
                users.AddPermission(Delete);
            });
            system.DefineGroup("Roles", roles =>
            {
                roles.AddPermission(View);
                roles.AddPermission(Create);
                roles.AddPermission(Edit);
                roles.AddPermission(Delete);
            });
            system.DefineGroup("Menus", menus =>
            {
                menus.AddPermission(View);
                menus.AddPermission(Create);
                menus.AddPermission(Edit);
                menus.AddPermission(Delete);
            });
        });
    }
}