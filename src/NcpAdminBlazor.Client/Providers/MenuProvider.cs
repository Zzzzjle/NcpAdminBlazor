using MudBlazor;
using NcpAdminBlazor.Client.Models;
using NcpAdminBlazor.Client.Pages.Applications.Chat;
using NcpAdminBlazor.Client.Pages.Applications.Email;
using NcpAdminBlazor.Client.Pages.Pages.Utility;
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
            .AddGroup("概览", Icons.Material.Filled.Settings, settings =>
            {
                settings.AddLink("仪表盘", Dashboard.PageUri);
            })
            .AddGroup("App Examples", Icons.Material.Filled.ShoppingBag, products =>
            {
                products.AddLink("Email", Email.PageUri,Icons.Material.Outlined.Email);
                products.AddLink("Chat", Chat.PageUri, Icons.Material.Outlined.Forum);
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