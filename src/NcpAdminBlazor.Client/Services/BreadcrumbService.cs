using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace NcpAdminBlazor.Client.Services;

public sealed class BreadcrumbService : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly MenuProvider _menuProvider;

    public IReadOnlyList<BreadcrumbItem> CurrentItems { get; private set; } = [];
    public event Action<List<BreadcrumbItem>>? OnBreadcrumbsChanged;

    public BreadcrumbService(NavigationManager navigationManager, MenuProvider menuProvider)
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
        var currentMenuItem = _menuProvider.FindMenuItemByHref(currentUri);

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

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }
}