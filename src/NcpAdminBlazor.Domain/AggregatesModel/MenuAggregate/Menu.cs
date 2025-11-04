using NcpAdminBlazor.Domain.Common;
using NcpAdminBlazor.Domain.DomainEvents;

namespace NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

public partial record MenuId : IGuidStronglyTypedId
{
    public static MenuId Root => new(Guid.Empty);
}

public class Menu : Entity<MenuId>, IAggregateRoot, ISoftDeletable
{
    protected Menu()
    {
    }

    public MenuId ParentId { get; private set; } = MenuId.Root;
    public string Title { get; private set; } = string.Empty;
    public string Icon { get; private set; } = string.Empty;
    public string PageKey { get; private set; } = string.Empty;
    public string Path { get; private set; } = string.Empty;
    public MenuType Type { get; private set; }
    public int Order { get; private set; }
    public bool IsDisable { get; private set; }
    public string PermissionCode { get; private set; } = string.Empty;
    public Deleted IsDeleted { get; private set; } = false;
    public DeletedTime DeletedAt { get; private set; } = new(DateTimeOffset.MinValue);

    public Menu(
        MenuId parentId,
        string title,
        MenuType type,
        int order,
        bool isDisable,
        string icon,
        string pageKey,
        string path,
        string permissionCode)
    {
        ParentId = parentId;
        Title = title;
        Icon = icon;
        Type = type;
        Order = order;
        IsDisable = isDisable;
        Path = path;
        PageKey = pageKey;
        PermissionCode = permissionCode;
    }

    public void Update(
        MenuId parentId,
        string title,
        MenuType type,
        int order,
        bool isDisable,
        string icon,
        string pageKey,
        string path,
        string permissionCode)
    {
        EnsureParentNotSelf(parentId);
        ParentId = parentId;
        Title = title;
        Icon = icon;
        Type = type;
        Order = order;
        IsDisable = isDisable;
        Path = path;
        PageKey = pageKey;
        PermissionCode = permissionCode;
        AddDomainEvent(new MenuUpdatedDomainEvent(this));
    }

    public void Delete()
    {
        IsDeleted = true;
        AddDomainEvent(new MenuDeletedDomainEvent(this));
    }

    private void EnsureParentNotSelf(MenuId? parentId)
    {
        if (parentId is null)
        {
            return;
        }

        if (Id != null && parentId == Id)
        {
            throw new KnownException("菜单不能设置自身为父级");
        }
    }
}