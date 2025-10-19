namespace NcpAdminBlazor.Client.Models;

public class MenuItem
{
    public required string Title { get; init; }
    public string? Href { get; init; }
    public string? Icon { get; init; }

#pragma warning disable S2325
    public List<MenuItem>? ChildItems
#pragma warning restore S2325
    {
        get;
        set
        {
            if (field != null)
                throw new InvalidOperationException("ChildItems already set and cannot be modified.");
            field = value;
        }
    }

    // 为了方便回溯，我们可以添加一个父级引用
    // 这个属性可以在服务初始化时填充
    public MenuItem? Parent { get; init; }
}