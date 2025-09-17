using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
// ReSharper disable VirtualMemberCallInConstructor

namespace NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate
{
    public partial record ApplicationUserId : IInt64StronglyTypedId;

    public class ApplicationUser : Entity<ApplicationUserId>, IAggregateRoot
    {
        protected ApplicationUser()
        {
        }

        public string Name { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string PasswordSalt { get; private set; } = string.Empty;

        public string RealName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        /// <summary>
        /// 0:已禁用  1:已启用
        /// </summary>
        public int Status { get; private set; } = 1;

        public DateTimeOffset CreatedAt { get; init; }
        public virtual ICollection<ApplicationUserRole> Roles { get; } = [];

        public virtual ICollection<ApplicationUserPermission> Permissions { get; } = [];
        public bool IsDeleted { get; private set; }
        public DateTimeOffset? DeletedAt { get; private set; }

        public ApplicationUser(string name, string phone, string passwordHash,
            string passwordSalt,
            IEnumerable<ApplicationUserRole> roles,
            IEnumerable<ApplicationUserPermission> permissions,
            string realName,
            int status, 
            string email)
        {
            CreatedAt = DateTimeOffset.Now;
            Name = name;
            Phone = phone;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            RealName = realName;
            Status = status;
            Email = email;
            foreach (var adminUserRole in roles)
            {
                Roles.Add(adminUserRole);
            }

            foreach (var adminUserPermission in permissions)
            {
                Permissions.Add(adminUserPermission);
            }
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

        public void SetSpecificPermissions(IEnumerable<ApplicationUserPermission> permissionsToBeAssigned)
        {
            var currentSpecificPermissionMap =
                Permissions.Where(p => p.SourceRoleIds.Count == 0).ToDictionary(p => p.PermissionCode);
            var newSpecificPermissionMap = permissionsToBeAssigned.ToDictionary(p => p.PermissionCode);

            var permissionCodesToRemove = currentSpecificPermissionMap.Keys.Except(newSpecificPermissionMap.Keys);
            foreach (var permissionCode in permissionCodesToRemove)
            {
                var permission = currentSpecificPermissionMap[permissionCode];
                Permissions.Remove(permission);
            }

            var permissionCodesToAdd = newSpecificPermissionMap.Keys.Except(currentSpecificPermissionMap.Keys);
            foreach (var permissionCode in permissionCodesToAdd)
            {
                if (Permissions.Any(p => p.PermissionCode == permissionCode))
                    throw new KnownException("权限重复！");
                Permissions.Add(newSpecificPermissionMap[permissionCode]);
            }
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
        }
        
        public void Delete()
        {
            if (IsDeleted) throw new KnownException("用户已经被删除！");
            IsDeleted = true;
            DeletedAt = DateTimeOffset.Now;
        }

    }
}