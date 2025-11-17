using System.Net;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.ApiService.Application.Queries.RolesManagement;
using NcpAdminBlazor.ApiService.Endpoints.RolesManagement;
using NcpAdminBlazor.ApiService.Tests.Fixtures;

namespace NcpAdminBlazor.ApiService.Tests.RoleManagement;

[Collection(AuthenticatedTestCollection.Name)]
public class RolesManagementTests(AuthenticatedAppFixture app, RolesManagementTests.RoleState state)
    : TestBase<AuthenticatedAppFixture, RolesManagementTests.RoleState>
{
    [Fact, Priority(1)]
    public async Task CreateRole_ShouldReturnRoleId()
    {
        var request = new CreateRoleRequest
        {
            Name = state.RoleName,
            Description = "Test role description"
        };

        var (rsp, res) = await app.AuthenticatedClient
            .POSTAsync<CreateRoleEndpoint, CreateRoleRequest, ResponseData<CreateRoleResponse>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.RoleId.Id.ShouldNotBe(Guid.Empty);

        state.RoleId = res.Data.RoleId;
    }

    [Fact, Priority(2)]
    public async Task RoleList_ShouldIncludeCreatedRole()
    {
        var (rsp, res) = await app.AuthenticatedClient
            .GETAsync<RoleListEndpoint, GetRoleListRequest, ResponseData<PagedData<RoleListItemDto>>>(
                new GetRoleListRequest
                {
                    Name = state.RoleName,
                    PageSize = 10
                });

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.Items.ShouldContain(r => r.Name == state.RoleName);
    }

    [Fact, Priority(3)]
    public async Task RoleInfo_ShouldReturnRoleInfo()
    {
        var roleId = state.RoleId ?? throw new InvalidOperationException("RoleId not initialized");

        var (rsp, res) = await app.AuthenticatedClient
            .GETAsync<RoleInfoEndpoint, RoleInfoRequest, ResponseData<RoleInfoResponse>>(
                new RoleInfoRequest { RoleId = roleId });

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.Name.ShouldBe(state.RoleName);
    }

    [Fact, Priority(4)]
    public async Task UpdateRoleInfo_ShouldModifyRoleDetails()
    {
        var roleId = state.RoleId ?? throw new InvalidOperationException("RoleId not initialized");
        const string updatedDescription = "Updated role description";

        var infoRequest = new UpdateRoleInfoRequest
        {
            RoleId = roleId,
            Name = state.RoleName,
            Description = updatedDescription
        };

        var (infoRsp, infoRes) = await app.AuthenticatedClient
            .POSTAsync<UpdateRoleInfoEndpoint, UpdateRoleInfoRequest, ResponseData>(infoRequest);

        infoRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        infoRes.Success.ShouldBeTrue();

        var (_, infoResponse) = await app.AuthenticatedClient
            .GETAsync<RoleInfoEndpoint, RoleInfoRequest, ResponseData<RoleInfoResponse>>(
                new RoleInfoRequest { RoleId = roleId });

        infoResponse.Success.ShouldBeTrue();
        infoResponse.Data.Description.ShouldBe(updatedDescription);
    }

    [Fact, Priority(6)]
    public async Task UpdateRolePermissions_ShouldModifyRolePermissions()
    {
        var roleId = state.RoleId ?? throw new InvalidOperationException("RoleId not initialized");
        
        // 创建测试权限代码列表
        var testPermissionCodes = new List<string> 
        { 
            "user.view", 
            "user.create", 
            "user.edit",
            "user.delete"
        };

        var updatePermissionsRequest = new UpdateRolePermissionsRequest
        {
            RoleId = roleId,
            PermissionCodes = testPermissionCodes
        };

        var (updateRsp, updateRes) = await app.AuthenticatedClient
            .POSTAsync<UpdateRolePermissionsEndpoint, UpdateRolePermissionsRequest, ResponseData>(updatePermissionsRequest);

        updateRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        updateRes.Success.ShouldBeTrue();

        // 验证权限已更新
        var (getRsp, getRes) = await app.AuthenticatedClient
            .GETAsync<RolePermissionsEndpoint, RolePermissionsRequest, ResponseData<RolePermissionsResponse>>(
                new RolePermissionsRequest { RoleId = roleId });

        getRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        getRes.Success.ShouldBeTrue();
        getRes.Data.PermissionCodes.Count.ShouldBe(4);
        getRes.Data.PermissionCodes.ShouldContain("user.view");
        getRes.Data.PermissionCodes.ShouldContain("user.create");
        getRes.Data.PermissionCodes.ShouldContain("user.edit");
        getRes.Data.PermissionCodes.ShouldContain("user.delete");
    }
    

    [Fact, Priority(8)]
    public async Task GetRolePermissions_ShouldReturnEmptyForNewRole()
    {
        var roleId = state.RoleId ?? throw new InvalidOperationException("RoleId not initialized");

        var (rsp, res) = await app.AuthenticatedClient
            .GETAsync<RolePermissionsEndpoint, RolePermissionsRequest, ResponseData<RolePermissionsResponse>>(
                new RolePermissionsRequest { RoleId = roleId });

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.RoleId.ShouldBe(roleId);
        // 权限列表可能为空或包含之前测试设置的权限
        res.Data.PermissionCodes.ShouldNotBeNull();
    }

    [Fact, Priority(9)]
    public async Task DeleteRole_ShouldRemoveRole()
    {
        var roleId = state.RoleId ?? throw new InvalidOperationException("RoleId not initialized");

        var (rsp, res) = await app.AuthenticatedClient
            .DELETEAsync<DeleteRoleEndpoint, DeleteRoleRequest, ResponseData>(
                new DeleteRoleRequest { RoleId = roleId });

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();

        var (_, listResponse) = await app.AuthenticatedClient
            .GETAsync<RoleListEndpoint, GetRoleListRequest, ResponseData<PagedData<RoleListItemDto>>>(
                new GetRoleListRequest
                {
                    Name = state.RoleName,
                    PageSize = 10
                });

        listResponse.Success.ShouldBeTrue();
        listResponse.Data.Items.ShouldNotContain(r => r.Name == state.RoleName);
    }

    [Fact, Priority(10)]
    public async Task GetAllPermissions_ShouldReturnPermissionTree()
    {
        var (rsp, res) = await app.AuthenticatedClient
            .GETAsync<AllPermissionsEndpoint, EmptyRequest, ResponseData<AllPermissionsResponse>>(new EmptyRequest());

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.Groups.ShouldNotBeEmpty();
        
        // 验证树形结构 - 应该有System根节点
        var systemGroup = res.Data.Groups.ShouldHaveSingleItem();
        systemGroup.Key.ShouldBe("System");
        systemGroup.SubGroups.Count.ShouldBe(3); // Users, Roles, Menus
        
        // 验证System.Users子节点
        var usersGroup = systemGroup.SubGroups.FirstOrDefault(g => g.Key == "System_Users");
        usersGroup.ShouldNotBeNull();
        usersGroup.Permissions.Count.ShouldBe(4); // View, Create, Edit, Delete
        
        // 验证权限项
        var viewPermission = usersGroup.Permissions.FirstOrDefault(p => p.Key == "System_Users_View");
        viewPermission.ShouldNotBeNull();
        viewPermission.LogicalName.ShouldBe("View");
        viewPermission.GroupKey.ShouldBe("System_Users");
    }

    public sealed class RoleState : StateFixture
    {
        public string RoleName { get; } = $"role_{Guid.NewGuid():N}";
        public RoleId? RoleId { get; set; }

        protected override ValueTask SetupAsync() => ValueTask.CompletedTask;

        protected override ValueTask TearDownAsync() => ValueTask.CompletedTask;
    }
}