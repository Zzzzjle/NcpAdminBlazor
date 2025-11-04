using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.Common;
using NcpAdminBlazor.Domain.DomainEvents;

namespace NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate
{
    public partial record RoleId : IGuidStronglyTypedId;

    public class Role : Entity<RoleId>, IAggregateRoot, ISoftDeletable
    {
        protected Role()
        {
        }

        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsDisabled { get; private set; } = false;
        public ICollection<MenuId> AssignedMenuIds { get; private set; } = [];
        public DateTimeOffset CreatedAt { get; init; }
        public Deleted IsDeleted { get; private set; } = false;
        public DeletedTime DeletedAt { get; private set; } = new(DateTimeOffset.MinValue);

        public Role(string name, string description, bool isDisabled)
        {
            CreatedAt = DateTimeOffset.Now;
            Name = name;
            Description = description;
            IsDisabled = isDisabled;
            AssignedMenuIds = [];
        }

        public void UpdateRoleInfo(string name, string description, bool isDisabled)
        {
            Name = name;
            Description = description;
            IsDisabled = isDisabled;
            AddDomainEvent(new RoleInfoChangedDomainEvent(this));
        }

        public void UpdateMenus(ICollection<MenuId> menuIds)
        {
            AssignedMenuIds = menuIds;
            AddDomainEvent(new RoleMenusChangedDomainEvent(this));
        }

        public void Delete()
        {
            IsDeleted = true;
            AddDomainEvent(new RoleDeletedDomainEvent(this));
        }
    }
}