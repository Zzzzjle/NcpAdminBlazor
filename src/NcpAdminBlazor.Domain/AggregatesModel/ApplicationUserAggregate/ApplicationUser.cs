using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Domain.Common;
using NcpAdminBlazor.Domain.DomainEvents;
using static NcpAdminBlazor.Domain.Common.PasswordHasher;

namespace NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate
{
    public partial record ApplicationUserId : IGuidStronglyTypedId;

    public class ApplicationUser : Entity<ApplicationUserId>, IAggregateRoot, ISoftDeletable
    {
        protected ApplicationUser()
        {
        }

        public string Username { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string PasswordSalt { get; private set; } = string.Empty;
        public string RealName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string RefreshToken { get; private set; } = string.Empty;
        public DateTimeOffset RefreshExpiry { get; private set; } = DateTimeOffset.MinValue;
        public ICollection<UserRole> Roles { get; private set; } = [];
        public ICollection<UserMenuPermission> MenuPermissions { get; private set; } = [];
        public DateTimeOffset CreatedAt { get; init; }
        public Deleted IsDeleted { get; private set; } = false;
        public DeletedTime DeletedAt { get; private set; } = new(DateTimeOffset.MinValue);

        public ApplicationUser(
            string username,
            string password,
            string realName,
            string email,
            string phone,
            List<UserRole> roles,
            List<UserMenuPermission> menuPermissions)
        {
            CreatedAt = DateTimeOffset.Now;
            Username = username;
            RealName = realName;
            Email = email;
            Phone = phone;
            var salt = GeneratePasswordSalt();
            var passwordHash = GeneratePasswordHash(password, salt);
            PasswordSalt = salt;
            PasswordHash = passwordHash;
            SetRoles(roles);
            SetMenuPermissions(menuPermissions);
            AddDomainEvent(new UserCreatedDomainEvent(this));
        }

        public void SyncRoleInfo(RoleId roleId, string roleName, bool isDisable,
            IEnumerable<UserMenuPermission>? menuPermissions = null)
        {
            var savedRole = Roles.FirstOrDefault(r => r.RoleId == roleId);
            if (savedRole is null)
            {
                var newRole = new UserRole(roleId, roleName);
                newRole.UpdateUserRoleInfo(roleName, isDisable);
                Roles.Add(newRole);
            }
            else
            {
                savedRole.UpdateUserRoleInfo(roleName, isDisable);
            }

            if (menuPermissions is not null)
            {
                ReplacePermissionsForRole(roleId, menuPermissions);
            }

            AddDomainEvent(new UserMenuPermissionsChanged(this));
        }

        public void RemoveRole(RoleId roleId)
        {
            var role = Roles.FirstOrDefault(r => r.RoleId == roleId);
            if (role is null) return;
            Roles.Remove(role);
            MenuPermissions = MenuPermissions
                .Where(mp => mp.SourceRoleId != roleId)
                .ToList();

            AddDomainEvent(new UserMenuPermissionsChanged(this));
        }

        public void UpdateUserInfo(string username, string realName, string email, string phone,
            List<UserRole> rolesToBeAssigned, List<UserMenuPermission> menuPermissions)
        {
            Username = username;
            RealName = realName;
            Email = email;
            Phone = phone;
            SetRoles(rolesToBeAssigned);
            SetMenuPermissions(menuPermissions);
            AddDomainEvent(new UserMenuPermissionsChanged(this));
        }

        public void SyncRolePermissions(RoleId roleId, IEnumerable<UserMenuPermission> menuPermissions)
        {
            ReplacePermissionsForRole(roleId, menuPermissions);
            AddDomainEvent(new UserMenuPermissionsChanged(this));
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            var oldPasswordHash = GeneratePasswordHash(oldPassword, PasswordSalt);
            if (PasswordHash != oldPasswordHash) throw new KnownException("旧密码不正确");
            var newPasswordHash = GeneratePasswordHash(newPassword, PasswordSalt);
            if (PasswordHash == newPasswordHash) throw new KnownException("新密码不能与旧密码相同");
            PasswordHash = newPasswordHash;
            AddDomainEvent(new UserPasswordChangedDomainEvent(this));
        }

        public void Login(string password)
        {
            if (IsDeleted) throw new KnownException("用户名或密码不正确");
            var passwordHash = GeneratePasswordHash(password, PasswordSalt);
            if (PasswordHash != passwordHash) throw new KnownException("用户名或密码不正确");

            AddDomainEvent(new UserLoginDomainEvent(this));
        }

        public void SetRefreshToken(string refreshToken, DateTimeOffset refreshExpiry)
        {
            RefreshToken = refreshToken;
            RefreshExpiry = refreshExpiry;
        }

        public void Delete()
        {
            if (IsDeleted) throw new KnownException("用户已经被删除！");
            IsDeleted = true;
            AddDomainEvent(new UserDeletedDomainEvent(this));
        }

        private void SetRoles(IEnumerable<UserRole> roles)
        {
            Roles = (roles ?? Enumerable.Empty<UserRole>())
                .GroupBy(r => r.RoleId)
                .Select(g => g.Last())
                .ToList();
        }

        private void SetMenuPermissions(IEnumerable<UserMenuPermission> menuPermissions)
        {
            MenuPermissions = (menuPermissions ?? Enumerable.Empty<UserMenuPermission>())
                .GroupBy(p => new { p.MenuId, p.PermissionCode, p.SourceRoleId })
                .Select(g => g.Last())
                .ToList();
        }

        private void ReplacePermissionsForRole(RoleId roleId, IEnumerable<UserMenuPermission> menuPermissions)
        {
            var normalizedPermissions = (menuPermissions ?? Enumerable.Empty<UserMenuPermission>())
                .Select(permission => new UserMenuPermission(permission.MenuId, roleId, permission.PermissionCode))
                .GroupBy(p => new { p.MenuId, p.PermissionCode })
                .Select(g => g.Last())
                .ToList();

            var retainedPermissions = MenuPermissions
                .Where(p => p.SourceRoleId != roleId)
                .ToList();

            retainedPermissions.AddRange(normalizedPermissions);
            MenuPermissions = retainedPermissions;
        }
    }
}