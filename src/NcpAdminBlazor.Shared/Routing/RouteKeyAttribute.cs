namespace NcpAdminBlazor.Shared.Routing;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RouteKeyAttribute : Attribute
{
    public RouteKeyAttribute(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("RouteKey不能为空", nameof(key));
        }

        Key = key.Trim();
    }

    public string Key { get; }
}
