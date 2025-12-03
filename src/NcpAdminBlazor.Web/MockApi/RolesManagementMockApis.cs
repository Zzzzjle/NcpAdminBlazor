using Microsoft.AspNetCore.Mvc;
using NcpAdminBlazor.Client.Models;

namespace NcpAdminBlazor.Web.MockApi;

public static class RolesManagementMockApis
{
    public static RouteGroupBuilder MapRolesManagementMockApis(this RouteGroupBuilder group, MockDataStore dataStore)
    {
        // 获取角色列表
        group.MapGet("/api/roles", (
            [FromQuery] string? name,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10) =>
        {
            var allRoles = dataStore.GetRoles();

            // 简化的过滤逻辑
            var filteredRoles = allRoles.AsEnumerable();
            if (!string.IsNullOrEmpty(name))
                filteredRoles = filteredRoles.Where(r => r.Name?.Contains(name) == true);

            var totalCount = filteredRoles.Count();
            var pagedRoles = filteredRoles
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedData = new NetCorePalExtensionsDtoPagedDataOfRoleListItemDto
            {
                Items = pagedRoles,
                Total = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var responseData = new NetCorePalExtensionsDtoResponseDataOfPagedDataOfRoleListItemDto
            {
                Data = pagedData,
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 创建角色
        group.MapPost("/api/roles", ([FromBody] NcpAdminBlazorApiServiceEndpointsRolesManagementCreateRoleRequest request) =>
        {
            var roleId = dataStore.CreateRole(request.Name!, request.Description);

            var response = new NcpAdminBlazorApiServiceEndpointsRolesManagementCreateRoleResponse
            {
                RoleId = roleId
            };

            var responseData = new NetCorePalExtensionsDtoResponseDataOfCreateRoleResponse
            {
                Data = response,
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 获取角色详情
        group.MapGet("/api/roles/{roleId}/info", (string roleId) =>
        {
            var role = dataStore.GetRoleById(roleId);
            if (role == null)
            {
                var errorResponse = new NetCorePalExtensionsDtoResponseDataOfRoleInfoResponse
                {
                    Success = false,
                    Message = "角色不存在"
                };
                return Results.Ok(errorResponse);
            }

            var response = new NcpAdminBlazorApiServiceEndpointsRolesManagementRoleInfoResponse
            {
                RoleId = role.RoleId,
                Name = role.Name,
                Description = role.Description,
                IsDisabled = role.IsDisabled
            };

            var responseData = new NetCorePalExtensionsDtoResponseDataOfRoleInfoResponse
            {
                Data = response,
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 更新角色信息
        group.MapPost("/api/roles/{roleId}/info", (string roleId, [FromBody] NcpAdminBlazorApiServiceEndpointsRolesManagementUpdateRoleInfoRequest request) =>
        {
            var role = dataStore.GetRoleById(roleId);
            if (role == null)
            {
                var errorResponse = new NetCorePalExtensionsDtoResponseData
                {
                    Success = false,
                    Message = "角色不存在"
                };
                return Results.Ok(errorResponse);
            }

            dataStore.UpdateRole(roleId, request.Name!, request.Description);

            var responseData = new NetCorePalExtensionsDtoResponseData
            {
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 删除角色
        group.MapDelete("/api/roles/{roleId}", (string roleId) =>
        {
            var role = dataStore.GetRoleById(roleId);
            if (role == null)
            {
                var errorResponse = new NetCorePalExtensionsDtoResponseData
                {
                    Success = false,
                    Message = "角色不存在"
                };
                return Results.Ok(errorResponse);
            }

            dataStore.DeleteRole(roleId);

            var responseData = new NetCorePalExtensionsDtoResponseData
            {
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 获取角色权限
        group.MapGet("/api/roles/{roleId}/permissions", (string roleId) =>
        {
            var role = dataStore.GetRoleById(roleId);
            if (role == null)
            {
                var errorResponse = new NetCorePalExtensionsDtoResponseDataOfRolePermissionsResponse
                {
                    Success = false,
                    Message = "角色不存在"
                };
                return Results.Ok(errorResponse);
            }

            var permissions = dataStore.GetRolePermissions(roleId);
            var response = new NcpAdminBlazorApiServiceEndpointsRolesManagementRolePermissionsResponse
            {
                RoleId = roleId,
                PermissionCodes = permissions
            };

            var responseData = new NetCorePalExtensionsDtoResponseDataOfRolePermissionsResponse
            {
                Data = response,
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 更新角色权限
        group.MapPost("/api/roles/{roleId}/permissions", (string roleId, [FromBody] NcpAdminBlazorApiServiceEndpointsRolesManagementUpdateRolePermissionsRequest request) =>
        {
            var role = dataStore.GetRoleById(roleId);
            if (role == null)
            {
                var errorResponse = new NetCorePalExtensionsDtoResponseData
                {
                    Success = false,
                    Message = "角色不存在"
                };
                return Results.Ok(errorResponse);
            }

            dataStore.UpdateRolePermissions(roleId, request.PermissionCodes?.ToList() ?? []);

            var responseData = new NetCorePalExtensionsDtoResponseData
            {
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 获取所有权限 (简化版本，返回简单的权限组结构)
        group.MapGet("/api/permissions", () =>
        {
            // 创建简化的权限组结构
            var permissionGroups = new List<NcpAdminBlazorApiServiceEndpointsRolesManagementPermissionGroupDto>
            {
                new()
                {
                    Key = "User",
                    LogicalName = "User",
                    DisplayName = "用户管理",
                    Description = "用户管理相关权限",
                    Permissions =
                    [
                        new() { Key = "User.View", LogicalName = "User.View", DisplayName = "查看用户", Description = "查看用户列表和详情", GroupKey = "User" },
                        new() { Key = "User.Create", LogicalName = "User.Create", DisplayName = "创建用户", Description = "创建新用户", GroupKey = "User" },
                        new() { Key = "User.Update", LogicalName = "User.Update", DisplayName = "更新用户", Description = "更新用户信息", GroupKey = "User" },
                        new() { Key = "User.Delete", LogicalName = "User.Delete", DisplayName = "删除用户", Description = "删除用户", GroupKey = "User" }
                    ],
                    SubGroups = []
                },
                new()
                {
                    Key = "Role",
                    LogicalName = "Role",
                    DisplayName = "角色管理",
                    Description = "角色管理相关权限",
                    Permissions =
                    [
                        new() { Key = "Role.View", LogicalName = "Role.View", DisplayName = "查看角色", Description = "查看角色列表和详情", GroupKey = "Role" },
                        new() { Key = "Role.Create", LogicalName = "Role.Create", DisplayName = "创建角色", Description = "创建新角色", GroupKey = "Role" },
                        new() { Key = "Role.Update", LogicalName = "Role.Update", DisplayName = "更新角色", Description = "更新角色信息", GroupKey = "Role" },
                        new() { Key = "Role.Delete", LogicalName = "Role.Delete", DisplayName = "删除角色", Description = "删除角色", GroupKey = "Role" }
                    ],
                    SubGroups = []
                }
            };

            var response = new NcpAdminBlazorApiServiceEndpointsRolesManagementAllPermissionsResponse
            {
                Groups = permissionGroups
            };

            var responseData = new NetCorePalExtensionsDtoResponseDataOfAllPermissionsResponse
            {
                Data = response,
                Success = true
            };

            return Results.Ok(responseData);
        });

        return group;
    }
}

