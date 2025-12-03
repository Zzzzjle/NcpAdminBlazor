using NcpAdminBlazor.Client.Models;

namespace NcpAdminBlazor.Web.MockApi;

/// <summary>
/// 模拟数据存储，用于Demo模式
/// </summary>
public class MockDataStore
{
    private readonly List<NcpAdminBlazorApiServiceApplicationQueriesUsersManagementUserListItemDto> _users = [];
    private readonly List<NcpAdminBlazorApiServiceApplicationQueriesRolesManagementRoleListItemDto> _roles = [];
    private readonly Dictionary<string, List<string>> _userRoles = [];
    private readonly Dictionary<string, List<string>> _rolePermissions = [];
    private int _nextUserId = 1;
    private int _nextRoleId = 1;

    public MockDataStore()
    {
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        // 初始化管理员用户
        var adminUserId = _nextUserId++.ToString();
        var adminUser = new NcpAdminBlazorApiServiceApplicationQueriesUsersManagementUserListItemDto
        {
            Id = adminUserId,
            Username = "admin",
            RealName = "系统管理员",
            Phone = "13800138000",
            Email = "admin@example.com",
            CreatedAt = DateTimeOffset.Now,
            RoleIds = ["1"]
        };
        _users.Add(adminUser);
        _userRoles[adminUserId] = ["1"];

        // 初始化普通用户
        var normalUserId = _nextUserId++.ToString();
        var normalUser = new NcpAdminBlazorApiServiceApplicationQueriesUsersManagementUserListItemDto
        {
            Id = normalUserId,
            Username = "user",
            RealName = "普通用户",
            Phone = "13900139000",
            Email = "user@example.com",
            CreatedAt = DateTimeOffset.Now,
            RoleIds = ["2"]
        };
        _users.Add(normalUser);
        _userRoles[normalUserId] = ["2"];

        // 初始化管理员角色
        var adminRoleId = _nextRoleId++.ToString();
        var adminRole = new NcpAdminBlazorApiServiceApplicationQueriesRolesManagementRoleListItemDto
        {
            RoleId = adminRoleId,
            Name = "管理员",
            Description = "系统管理员角色",
            CreatedAt = DateTimeOffset.Now,
            IsDisabled = false
        };
        _roles.Add(adminRole);
        _rolePermissions[adminRoleId] = ["User.View", "User.Create", "User.Update", "User.Delete", 
                                         "Role.View", "Role.Create", "Role.Update", "Role.Delete"];

        // 初始化普通用户角色
        var userRoleId = _nextRoleId++.ToString();
        var userRole = new NcpAdminBlazorApiServiceApplicationQueriesRolesManagementRoleListItemDto
        {
            RoleId = userRoleId,
            Name = "普通用户",
            Description = "普通用户角色",
            CreatedAt = DateTimeOffset.Now,
            IsDisabled = false
        };
        _roles.Add(userRole);
        _rolePermissions[userRoleId] = ["User.View"];
    }

    // 用户相关方法
    public List<NcpAdminBlazorApiServiceApplicationQueriesUsersManagementUserListItemDto> GetUsers() => _users;

    public NcpAdminBlazorApiServiceApplicationQueriesUsersManagementUserListItemDto? GetUserById(string id) =>
        _users.FirstOrDefault(u => u.Id == id);

    public NcpAdminBlazorApiServiceApplicationQueriesUsersManagementUserListItemDto? GetUserByPhone(string phone) =>
        _users.FirstOrDefault(u => u.Phone == phone);

    public NcpAdminBlazorApiServiceApplicationQueriesUsersManagementUserListItemDto? GetUserByUsername(string username) =>
        _users.FirstOrDefault(u => u.Username == username);

    public string CreateUser(string username, string realName, string phone, string? email, List<string> roleIds)
    {
        var userId = _nextUserId++.ToString();
        var user = new NcpAdminBlazorApiServiceApplicationQueriesUsersManagementUserListItemDto
        {
            Id = userId,
            Username = username,
            RealName = realName,
            Phone = phone,
            Email = email,
            CreatedAt = DateTimeOffset.Now,
            RoleIds = roleIds
        };
        _users.Add(user);
        _userRoles[userId] = roleIds;
        return userId;
    }

    public void UpdateUser(string id, string username, string realName, string phone, string? email)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            user.Username = username;
            user.RealName = realName;
            user.Phone = phone;
            user.Email = email;
        }
    }

    public void DeleteUser(string id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            _users.Remove(user);
            _userRoles.Remove(id);
        }
    }

    public List<string> GetUserRoleIds(string userId) =>
        _userRoles.TryGetValue(userId, out var roleIds) ? roleIds : [];

    // 角色相关方法
    public List<NcpAdminBlazorApiServiceApplicationQueriesRolesManagementRoleListItemDto> GetRoles() => _roles;

    public NcpAdminBlazorApiServiceApplicationQueriesRolesManagementRoleListItemDto? GetRoleById(string id) =>
        _roles.FirstOrDefault(r => r.RoleId == id);

    public string CreateRole(string name, string? description)
    {
        var roleId = _nextRoleId++.ToString();
        var role = new NcpAdminBlazorApiServiceApplicationQueriesRolesManagementRoleListItemDto
        {
            RoleId = roleId,
            Name = name,
            Description = description,
            CreatedAt = DateTimeOffset.Now,
            IsDisabled = false
        };
        _roles.Add(role);
        _rolePermissions[roleId] = [];
        return roleId;
    }

    public void UpdateRole(string id, string name, string? description)
    {
        var role = _roles.FirstOrDefault(r => r.RoleId == id);
        if (role != null)
        {
            role.Name = name;
            role.Description = description;
        }
    }

    public void DeleteRole(string id)
    {
        var role = _roles.FirstOrDefault(r => r.RoleId == id);
        if (role != null)
        {
            _roles.Remove(role);
            _rolePermissions.Remove(id);
            
            // 从所有用户中移除该角色
            foreach (var userRoleIds in _userRoles.Values)
            {
                userRoleIds.Remove(id);
            }
        }
    }

    public List<string> GetRolePermissions(string roleId) =>
        _rolePermissions.TryGetValue(roleId, out var permissions) ? permissions : [];

    public void UpdateRolePermissions(string roleId, List<string> permissions)
    {
        _rolePermissions[roleId] = permissions;
    }
}

