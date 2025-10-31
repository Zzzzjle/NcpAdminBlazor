using System.Linq;
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
        public ICollection<RoleMenuPermission> MenuPermissions { get; private set; } = [];
        public DateTimeOffset CreatedAt { get; init; }
        public Deleted IsDeleted { get; private set; } = false;
        public DeletedTime DeletedAt { get; private set; } = new(DateTimeOffset.MinValue);

        public Role(string name, string description, IEnumerable<RoleMenuPermission> permissions)
        {
            CreatedAt = DateTimeOffset.Now;
            Name = name;
            Description = description;
            SetPermissions(permissions);
        }

        public void UpdateRoleInfo(string name, string description, bool? isDisabled = null)
        {
            Name = name;
            Description = description;
            if (isDisabled.HasValue)
            {
                IsDisabled = isDisabled.Value;
            }
            AddDomainEvent(new RoleInfoChangedDomainEvent(this));
        }

        public void UpdatePermissions(IEnumerable<RoleMenuPermission> permissions)
        {
            SetPermissions(permissions);
            AddDomainEvent(new RolePermissionChangedDomainEvent(this));
        }

        public void Delete()
        {
            IsDeleted = true;
            AddDomainEvent(new RoleDeletedDomainEvent(this));
        }

        private void SetPermissions(IEnumerable<RoleMenuPermission> permissions)
        {
            MenuPermissions = permissions
                .GroupBy(p => new { p.MenuId, p.PermissionCode })
                .Select(g => g.Last())
                .ToList();
        }
    }
}