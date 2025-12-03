using Microsoft.AspNetCore.Mvc;
using NcpAdminBlazor.Client.Models;

namespace NcpAdminBlazor.Web.MockApi;

public static class UsersManagementMockApis
{
    public static RouteGroupBuilder MapUsersManagementMockApis(this RouteGroupBuilder group, MockDataStore dataStore)
    {
        // 用户登录
        group.MapPost("/api/user/login", ([FromBody] NcpAdminBlazorApiServiceEndpointsUsersManagementLoginRequest request) =>
        {
            var user = dataStore.GetUserByUsername(request.Username!);
            if (user == null || request.Password != "123456") // 简化处理，所有用户密码都是123456
            {
                // 返回失败的 ResponseData，而不是 BadRequest
                var errorResponse = new NetCorePalExtensionsDtoResponseDataOfMyTokenResponse
                {
                    Success = false,
                    Message = "用户名或密码错误"
                };
                return Results.Ok(errorResponse);
            }

            var response = new NcpAdminBlazorApiServiceAspNetCoreMyTokenResponse
            {
                AccessToken = $"mock_access_token_{user.Id}",
                RefreshToken = $"mock_refresh_token_{user.Id}",
                UserId = user.Id,
                AccessTokenExpiry = DateTimeOffset.Now.AddHours(2),
                RefreshTokenExpiry = DateTimeOffset.Now.AddDays(7)
            };

            var responseData = new NetCorePalExtensionsDtoResponseDataOfMyTokenResponse
            {
                Data = response,
                Success = true
            };

            return Results.Ok(responseData);
        })
        .AllowAnonymous();

        // 用户注册 (简化版,只需要用户名和密码)
        group.MapPost("/api/user/create", ([FromBody] NcpAdminBlazorApiServiceEndpointsUsersManagementRegisterUserRequest request) =>
        {
            var existingUser = dataStore.GetUserByUsername(request.Username!);
            if (existingUser != null)
            {
                var errorResponse = new NetCorePalExtensionsDtoResponseDataOfRegisterUserResponse
                {
                    Success = false,
                    Message = "用户名已存在"
                };
                return Results.Ok(errorResponse);
            }

            var userId = dataStore.CreateUser(
                request.Username!,
                request.Username!, // 使用用户名作为真实姓名
                "13900000000", // 默认手机号
                null,
                ["2"] // 默认普通用户角色
            );

            var response = new NcpAdminBlazorApiServiceEndpointsUsersManagementRegisterUserResponse
            {
                UserId = userId
            };

            var responseData = new NetCorePalExtensionsDtoResponseDataOfRegisterUserResponse
            {
                Data = response,
                Success = true
            };

            return Results.Ok(responseData);
        })
        .AllowAnonymous();

        // 获取用户列表
        group.MapGet("/api/user/list", (
            [FromQuery] string? username,
            [FromQuery] string? email,
            [FromQuery] string? phone,
            [FromQuery] string? realName,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10) =>
        {
            var allUsers = dataStore.GetUsers();

            // 简化的过滤逻辑
            var filteredUsers = allUsers.AsEnumerable();
            if (!string.IsNullOrEmpty(username))
                filteredUsers = filteredUsers.Where(u => u.Username?.Contains(username) == true);
            if (!string.IsNullOrEmpty(email))
                filteredUsers = filteredUsers.Where(u => u.Email?.Contains(email) == true);
            if (!string.IsNullOrEmpty(phone))
                filteredUsers = filteredUsers.Where(u => u.Phone?.Contains(phone) == true);
            if (!string.IsNullOrEmpty(realName))
                filteredUsers = filteredUsers.Where(u => u.RealName?.Contains(realName) == true);

            var totalCount = filteredUsers.Count();
            var pagedUsers = filteredUsers
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedData = new NetCorePalExtensionsDtoPagedDataOfUserListItemDto
            {
                Items = pagedUsers,
                Total = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var responseData = new NetCorePalExtensionsDtoResponseDataOfPagedDataOfUserListItemDto
            {
                Data = pagedData,
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 创建用户
        group.MapPost("/api/users", ([FromBody] NcpAdminBlazorApiServiceEndpointsUsersManagementCreateUserRequest request) =>
        {
            var existingUser = dataStore.GetUserByUsername(request.Username!);
            if (existingUser != null)
            {
                var errorResponse = new NetCorePalExtensionsDtoResponseDataOfCreateUserResponse
                {
                    Success = false,
                    Message = "用户名已存在"
                };
                return Results.Ok(errorResponse);
            }

            var userId = dataStore.CreateUser(
                request.Username!,
                request.RealName!,
                request.Phone!,
                request.Email,
                request.AssignedRoleIds?.ToList() ?? []
            );

            var response = new NcpAdminBlazorApiServiceEndpointsUsersManagementCreateUserResponse
            {
                UserId = userId
            };

            var responseData = new NetCorePalExtensionsDtoResponseDataOfCreateUserResponse
            {
                Data = response,
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 获取用户详情
        group.MapGet("/api/user/{userId}/profile", (string userId) =>
        {
            var user = dataStore.GetUserById(userId);
            if (user == null)
            {
                var errorResponse = new NetCorePalExtensionsDtoResponseDataOfUserInfoDto
                {
                    Success = false,
                    Message = "用户不存在"
                };
                return Results.Ok(errorResponse);
            }

            var roles = dataStore.GetRoles()
                .Where(r => user.RoleIds?.Contains(r.RoleId!) == true)
                .Select(r => new NcpAdminBlazorApiServiceApplicationQueriesUsersManagementUserRoleSummaryDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.Name
                })
                .ToList();

            var userInfo = new NcpAdminBlazorApiServiceApplicationQueriesUsersManagementUserInfoDto
            {
                Id = user.Id,
                Username = user.Username,
                RealName = user.RealName,
                Phone = user.Phone,
                Email = user.Email,
                Roles = roles,
                CreatedAt = user.CreatedAt
            };

            var responseData = new NetCorePalExtensionsDtoResponseDataOfUserInfoDto
            {
                Data = userInfo,
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 更新用户
        group.MapPost("/api/user/{userId}/update", (string userId, [FromBody] NcpAdminBlazorApiServiceEndpointsUsersManagementUpdateUserRequest request) =>
        {
            var user = dataStore.GetUserById(userId);
            if (user == null)
            {
                var errorResponse = new NetCorePalExtensionsDtoResponseData
                {
                    Success = false,
                    Message = "用户不存在"
                };
                return Results.Ok(errorResponse);
            }

            dataStore.UpdateUser(userId, request.Username!, request.RealName!, request.Phone!, request.Email);

            var responseData = new NetCorePalExtensionsDtoResponseData
            {
                Success = true
            };

            return Results.Ok(responseData);
        });

        // 删除用户
        group.MapDelete("/api/user/{userId}/delete", (string userId) =>
        {
            var user = dataStore.GetUserById(userId);
            if (user == null)
            {
                var errorResponse = new NetCorePalExtensionsDtoResponseData
                {
                    Success = false,
                    Message = "用户不存在"
                };
                return Results.Ok(errorResponse);
            }

            dataStore.DeleteUser(userId);

            var responseData = new NetCorePalExtensionsDtoResponseData
            {
                Success = true
            };

            return Results.Ok(responseData);
        });

        return group;
    }
}
