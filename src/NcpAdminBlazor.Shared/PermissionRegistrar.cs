using FluentPermissions.Core.Abstractions;
using FluentPermissions.Core.Builder;

namespace NcpAdminBlazor.Shared;

// ReSharper disable once UnusedType.Global
public class PermissionRegistrar : IPermissionRegistrar
{
    #region

    private const string Create = "Create";
    private const string Delete = "Delete";

    #endregion

    public void Register(PermissionBuilder builder)
    {
        builder.DefineGroup("System", system =>
        {
            system.DefineGroup("Users", users =>
            {
                users.AddPermission(Create);
                users.AddPermission(Delete);
            });
            system.DefineGroup("Roles", roles =>
            {
                roles.AddPermission(Create);
                roles.AddPermission(Delete);
            });
        });
    }
}