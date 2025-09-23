using MudBlazor;

namespace NcpAdminBlazor.Client.States;

public class BreadcrumbState
{
    // 用于存储当前面包屑项的私有字段
    private List<BreadcrumbItem> _items = [];

    /// <summary>
    /// 当面包屑项发生变化时触发的事件。
    /// 布局组件将订阅此事件以接收更新。
    /// </summary>
    public event Action<List<BreadcrumbItem>>? OnBreadcrumbsChanged;

    /// <summary>
    /// 获取当前的面包屑项列表。
    /// </summary>
    public IReadOnlyList<BreadcrumbItem> CurrentItems => _items.AsReadOnly();

    /// <summary>
    /// 设置新的面包屑项列表，并通知所有订阅者。
    /// 页面组件将调用此方法来更新面包屑。
    /// </summary>
    /// <param name="items">要显示的新面包屑项列表。</param>
    public void SetBreadcrumbs(List<BreadcrumbItem> items)
    {
        // 更新内部列表
        _items = items;

        // 触发事件，将新的列表传递给所有订阅者
        OnBreadcrumbsChanged?.Invoke(_items);
    }
}