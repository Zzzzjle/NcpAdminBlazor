// ReSharper disable once CheckNamespace

namespace NcpAdminBlazor.Shared.Auth;

public static partial class AppPermissions
{
    /// <summary>
    /// 获取全部权限Key
    /// </summary>
    public static IReadOnlyList<string> GetAllPermissionKeys()
    {
        return PermsByKey.Select(p => p.Key)
            .ToArray();
    }
}