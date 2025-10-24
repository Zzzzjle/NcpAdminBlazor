using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Domain.DomainEvents.User;

// ReSharper disable VirtualMemberCallInConstructor

namespace NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate
{
    public partial record ApplicationUserId : IInt64StronglyTypedId;

    public class ApplicationUser : Entity<ApplicationUserId>, IAggregateRoot
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

        public DateTimeOffset RefreshExpiry { get; private set; } = DateTimeOffset.MinValue;

        public string RefreshToken { get; private set; } = string.Empty;

        /// <summary>
        /// 0:已禁用  1:已启用
        /// </summary>
        public int Status { get; private set; } = 1;

        public DateTimeOffset CreatedAt { get; init; }
        public virtual ICollection<ApplicationUserRole> Roles { get; } = [];

        public virtual ICollection<ApplicationUserPermission> Permissions { get; } = [];

        public Deleted IsDeleted { get; private set; } = false;
        public DeletedTime DeletedAt { get; private set; } = new(DateTimeOffset.MinValue);

        public ApplicationUser(string username, string passwordHash,
            string passwordSalt)
        {
            CreatedAt = DateTimeOffset.Now;
            Username = username;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            // 发布用户创建领域事件
            AddDomainEvent(new ApplicationUserCreatedDomainEvent(this));
        }

        public void UpdateRoleInfo(RoleId roleId, string roleName)
        {
            var savedRole = Roles.FirstOrDefault(r => r.RoleId == roleId);
            savedRole?.UpdateRoleInfo(roleName);
        }

        public void UpdateRoles(IEnumerable<ApplicationUserRole> rolesToBeAssigned,
            IEnumerable<ApplicationUserPermission> permissions)
        {
            var currentRoleMap = Roles.ToDictionary(r => r.RoleId);
            var targetRoleMap = rolesToBeAssigned.ToDictionary(r => r.RoleId);

            var roleIdsToRemove = currentRoleMap.Keys.Except(targetRoleMap.Keys);
            foreach (var roleId in roleIdsToRemove)
            {
                Roles.Remove(currentRoleMap[roleId]);
                RemoveRolePermissions(roleId);
            }

            var roleIdsToAdd = targetRoleMap.Keys.Except(currentRoleMap.Keys);
            foreach (var roleId in roleIdsToAdd)
            {
                var targetRole = targetRoleMap[roleId];
                Roles.Add(targetRole);
            }

            AddPermissions(permissions);
        }

        public void UpdateRolePermissions(RoleId roleId, IEnumerable<ApplicationUserPermission> newPermissions)
        {
            RemoveRolePermissions(roleId);
            AddPermissions(newPermissions);
        }

        public void RemoveRole(RoleId roleId)
        {
            var role = Roles.FirstOrDefault(r => r.RoleId == roleId);
            if (role is null)
            {
                return;
            }

            Roles.Remove(role);
            RemoveRolePermissions(roleId);
            AddDomainEvent(new ApplicationUserInfoUpdatedDomainEvent(this));
        }

        private void AddPermissions(IEnumerable<ApplicationUserPermission> permissions)
        {
            foreach (var permission in permissions)
            {
                var existedPermission = Permissions.SingleOrDefault(p => p.PermissionCode == permission.PermissionCode);
                if (existedPermission is not null)
                {
                    foreach (var permissionSourceRoleId in permission.SourceRoleIds)
                        existedPermission.AddSourceRoleId(permissionSourceRoleId);
                }
                else
                {
                    Permissions.Add(permission);
                }
            }
        }

        private void RemoveRolePermissions(RoleId roleId)
        {
            foreach (var permission in Permissions.Where(p => p.SourceRoleIds.Remove(roleId) &&
                                                              p.SourceRoleIds.Count == 0)
                         .ToArray())
            {
                Permissions.Remove(permission);
            }
        }

        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="realName">真实姓名</param>
        /// <param name="email">邮箱</param>
        /// <param name="phone">手机号</param>
        /// <param name="status">状态标识</param>
        public void UpdateProfile(string username, string realName, string email, string phone, int status)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new KnownException("用户名不能为空");
            if (string.IsNullOrWhiteSpace(realName)) throw new KnownException("姓名不能为空");
            if (string.IsNullOrWhiteSpace(email)) throw new KnownException("邮箱不能为空");
            if (string.IsNullOrWhiteSpace(phone)) throw new KnownException("手机号不能为空");
            if (status is not (0 or 1)) throw new KnownException("用户状态无效");

            var normalizedUsername = username.Trim();
            var normalizedRealName = realName.Trim();
            var normalizedEmail = email.Trim();
            var normalizedPhone = phone.Trim();

            Username = normalizedUsername;
            RealName = normalizedRealName;
            Email = normalizedEmail;
            Phone = normalizedPhone;
            Status = status;

            AddDomainEvent(new ApplicationUserInfoUpdatedDomainEvent(this));
        }

        /// <summary>
        ///     修改密码
        /// </summary>
        /// <param name="oldPasswordHash"></param>
        /// <param name="newPasswordHash"></param>
        public void EditPassword(string oldPasswordHash, string newPasswordHash)
        {
            if (PasswordHash != oldPasswordHash) throw new KnownException("旧密码不正确");
            PasswordHash = newPasswordHash;
            AddDomainEvent(new ApplicationUserPasswordChangedDomainEvent(this));
        }

        /// <summary>
        /// 修改密码（包含盐值更新）
        /// </summary>
        /// <param name="oldPasswordHash">旧密码哈希</param>
        /// <param name="newPasswordHash">新密码哈希</param>
        /// <param name="newPasswordSalt">新密码盐值</param>
        public void ChangePassword(string oldPasswordHash, string newPasswordHash, string newPasswordSalt)
        {
            if (PasswordHash != oldPasswordHash) throw new KnownException("旧密码不正确");
            PasswordHash = newPasswordHash;
            PasswordSalt = newPasswordSalt;
            AddDomainEvent(new ApplicationUserPasswordChangedDomainEvent(this));
        }

        /// <summary>
        /// 验证用户登录
        /// </summary>
        /// <param name="passwordHash">密码哈希</param>
        /// <returns>登录是否成功</returns>
        public bool VerifyLogin(string passwordHash)
        {
            if (IsDeleted) throw new KnownException("用户已被删除，无法登录");
            if (Status != 1) throw new KnownException("用户已被禁用，无法登录");

            var loginSuccess = PasswordHash == passwordHash;
            if (loginSuccess)
            {
                AddDomainEvent(new ApplicationUserLoginDomainEvent(this));
            }

            return loginSuccess;
        }

        public void Delete()
        {
            if (IsDeleted) throw new KnownException("用户已经被删除！");
            IsDeleted = true;
            AddDomainEvent(new ApplicationUserDeletedDomainEvent(this));
        }

        /// <summary>
        /// 设置/更新刷新令牌
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <param name="refreshExpiry">刷新令牌到期时间</param>
        public void SetRefreshToken(string refreshToken, DateTimeOffset refreshExpiry)
        {
            RefreshToken = refreshToken;
            RefreshExpiry = refreshExpiry;
            AddDomainEvent(new ApplicationUserRefreshTokenUpdatedDomainEvent(this));
        }
    }
}