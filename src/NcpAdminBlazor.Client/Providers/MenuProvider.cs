using MudBlazor;
using NcpAdminBlazor.Client.Models;

namespace NcpAdminBlazor.Client.Providers;

public class MenuProvider
{
    private readonly List<MenuItem> _menuItems;

    public IReadOnlyList<MenuItem> MenuItems => _menuItems.AsReadOnly();

    public MenuProvider()
    {
        var builder = new MenuBuilder();

        builder
            .AddLink("首页", "/", Icons.Material.Filled.Home)
            .AddGroup("系统设置", Icons.Material.Filled.Settings, settings =>
            {
                // "settings" 这个 builder 在被创建时，内部已经持有了“系统设置”这个 MenuItem 作为父级
                settings.AddLink("通用设置", "/settings/general"); // 这个链接的 Parent 会被自动设为“系统设置”
                settings.AddLink("安全设置", "/settings/security"); // 同上
            })
            .AddGroup("产品管理", Icons.Material.Filled.ShoppingBag, products =>
            {
                products.AddLink("产品列表", "/products");
                products.AddLink("添加产品", "/products/add");
            });

        _menuItems = builder.Items;
    }
}

public class MenuBuilder(MenuItem? parent = null)
{
    public List<MenuItem> Items { get; } = [];

    public MenuBuilder AddLink(string title, string href, string? icon = null)
    {
        var linkItem = new MenuItem
        {
            Title = title,
            Href = href,
            Icon = icon,
            Parent = parent
        };
        Items.Add(linkItem);
        return this;
    }

    public MenuBuilder AddGroup(string title, string? icon, Action<MenuBuilder> configure)
    {
        var groupItem = new MenuItem
        {
            Title = title,
            Icon = icon,
            Parent = parent
        };

        var groupBuilder = new MenuBuilder(groupItem);

        configure(groupBuilder);
        groupItem.ChildItems = groupBuilder.Items;
        Items.Add(groupItem);

        return this;
    }
}