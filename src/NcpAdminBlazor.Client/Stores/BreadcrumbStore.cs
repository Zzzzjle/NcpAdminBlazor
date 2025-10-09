using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using NcpAdminBlazor.Client.Models;
using NcpAdminBlazor.Client.Providers;

namespace NcpAdminBlazor.Client.Stores;

public sealed class BreadcrumbStore : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly MenuProvider _menuProvider;

    public IReadOnlyList<BreadcrumbItem> CurrentItems { get; private set; } = [];
    public event Action<List<BreadcrumbItem>>? OnBreadcrumbsChanged;

    public BreadcrumbStore(NavigationManager navigationManager, MenuProvider menuProvider)
    {
        _navigationManager = navigationManager;
        _menuProvider = menuProvider;

        // 订阅路由变化事件
        _navigationManager.LocationChanged += OnLocationChanged;

        // 立即为当前页面生成一次面包屑
        GenerateBreadcrumbsForCurrentLocation();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        GenerateBreadcrumbsForCurrentLocation();
    }

    private void GenerateBreadcrumbsForCurrentLocation()
    {
        var currentUri = $"/{_navigationManager.ToBaseRelativePath(_navigationManager.Uri)}";
        var breadcrumbItems = new List<BreadcrumbItem>();

        // 核心步骤：在菜单树中查找匹配当前路径的节点
        var currentMenuItem = FindMenuItemByHref(_menuProvider.MenuItems, currentUri);

        if (currentMenuItem != null)
        {
            var node = currentMenuItem;
            // 核心步骤：通过 Parent 属性向上回溯，构建面包屑
            while (node != null)
            {
                breadcrumbItems.Insert(0,
                    new BreadcrumbItem(node.Title, href: node.Href, icon: node.Icon));
                node = node.Parent;
            }
        }

        CurrentItems = breadcrumbItems;
        OnBreadcrumbsChanged?.Invoke(breadcrumbItems);
    }

    // 使用深度优先搜索 (DFS) 来查找最深、最精确匹配的菜单项
    private static MenuItem? FindMenuItemByHref(IEnumerable<MenuItem> items, string href)
    {
        MenuItem? bestMatch = null;
        foreach (var item in items)
        {
            // 检查当前项是否是 href 的一个前缀
            // 例如: /products/details/123 匹配 /products/details
            if (item.Href != null && href.StartsWith(item.Href, StringComparison.OrdinalIgnoreCase) &&
                (bestMatch == null || item.Href.Length > bestMatch.Href!.Length))
            {
                // 如果当前项是更好的匹配项（更长的前缀），则更新 bestMatch
                bestMatch = item;
            }

            // 递归搜索子项，寻找更深、更精确的匹配
            if (item.ChildItems is null || item.ChildItems.Count == 0) continue;
            var childMatch = FindMenuItemByHref(item.ChildItems, href);
            if (childMatch != null && (bestMatch == null || childMatch.Href!.Length > bestMatch.Href!.Length))
            {
                // 如果子项的匹配度更高，则采纳子项的匹配
                bestMatch = childMatch;
            }
        }

        return bestMatch;
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }
}