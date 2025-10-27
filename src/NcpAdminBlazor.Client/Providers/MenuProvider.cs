using MudBlazor;
using NcpAdminBlazor.Client.Models;
using NcpAdminBlazor.Client.Pages;
using NcpAdminBlazor.Client.Pages.Applications.Email;
using NcpAdminBlazor.Client.Pages.Applications.Role;
using NcpAdminBlazor.Client.Pages.Applications.User;
using NcpAdminBlazor.Client.Pages.Personal;

namespace NcpAdminBlazor.Client.Providers;

public class MenuProvider
{
    private readonly List<MenuItem> _menuItems;

    public IReadOnlyList<MenuItem> MenuItems => _menuItems.AsReadOnly();

    public MenuProvider()
    {
        var builder = new MenuBuilder();

        builder
            .AddLink("首页", Home.PageUri, Icons.Material.Filled.Home)
            .AddGroup("系统管理", Icons.Material.Filled.Settings, system =>
            {
                system.AddLink("用户管理", UserList.PageUri, Icons.Material.Outlined.People);
                system.AddLink("角色管理", RoleList.PageUri, Icons.Material.Outlined.Security);
            })
            .AddGroup("App Examples", Icons.Material.Filled.ShoppingBag, products =>
            {
                products.AddLink("仪表盘", Dashboard.PageUri);
                products.AddLink("Email", Email.PageUri, Icons.Material.Outlined.Email);
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