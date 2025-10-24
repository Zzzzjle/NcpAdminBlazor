using System.Net;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Shared.Auth;
using NcpAdminBlazor.Web.Application.Queries.Roles;
using NcpAdminBlazor.Web.Endpoints.Roles;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests;

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
            Description = "Test role description",
            Status = 1,
            PermissionCodes = [AppPermissions.Keys.System_Roles_Create]
        };

        var (rsp, res) = await app.AuthenticatedClient
            .POSTAsync<CreateRoleEndpoint, CreateRoleRequest, ResponseData<CreateRoleResponse>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.RoleId.Id.ShouldBeGreaterThan(0);

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
            .GETAsync<RoleInfoEndpoint, RoleInfoRequest, ResponseData<RoleInfoDto>>(
                new RoleInfoRequest { RoleId = roleId });

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.Name.ShouldBe(state.RoleName);

        var (permRsp, permRes) = await app.AuthenticatedClient
            .GETAsync<RolePermissionsEndpoint, RolePermissionsRequest, ResponseData<RolePermissionsDto>>(
                new RolePermissionsRequest { RoleId = roleId });

        permRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        permRes.Success.ShouldBeTrue();
        permRes.Data.PermissionCodes.ShouldContain(AppPermissions.Keys.System_Roles_Create);
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
            Description = updatedDescription,
            Status = 0
        };

        var (infoRsp, infoRes) = await app.AuthenticatedClient
            .POSTAsync<UpdateRoleInfoEndpoint, UpdateRoleInfoRequest, ResponseData>(infoRequest);

        infoRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        infoRes.Success.ShouldBeTrue();

        var (_, infoResponse) = await app.AuthenticatedClient
            .GETAsync<RoleInfoEndpoint, RoleInfoRequest, ResponseData<RoleInfoDto>>(
                new RoleInfoRequest { RoleId = roleId });

        infoResponse.Success.ShouldBeTrue();
        infoResponse.Data.Description.ShouldBe(updatedDescription);
        infoResponse.Data.Status.ShouldBe(0);
    }

    [Fact, Priority(5)]
    public async Task UpdateRolePermissions_ShouldModifyRolePermissions()
    {
        var roleId = state.RoleId ?? throw new InvalidOperationException("RoleId not initialized");

        var permissionsRequest = new UpdateRolePermissionsRequest
        {
            RoleId = roleId,
            PermissionCodes = [AppPermissions.Keys.System_Roles_Delete]
        };

        var (permRsp, permRes) = await app.AuthenticatedClient
            .POSTAsync<UpdateRolePermissionsEndpoint, UpdateRolePermissionsRequest, ResponseData>(permissionsRequest);

        permRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        permRes.Success.ShouldBeTrue();

        var (_, permissionsResponse) = await app.AuthenticatedClient
            .GETAsync<RolePermissionsEndpoint, RolePermissionsRequest, ResponseData<RolePermissionsDto>>(
                new RolePermissionsRequest { RoleId = roleId });

        permissionsResponse.Success.ShouldBeTrue();
        permissionsResponse.Data.PermissionCodes.ShouldContain(AppPermissions.Keys.System_Roles_Delete);
        permissionsResponse.Data.PermissionCodes.ShouldNotContain(AppPermissions.Keys.System_Roles_Create);
    }

    [Fact, Priority(6)]
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

    public sealed class RoleState : StateFixture
    {
        public string RoleName { get; } = $"role_{Guid.NewGuid():N}";
        public RoleId? RoleId { get; set; }

        protected override ValueTask SetupAsync() => ValueTask.CompletedTask;

        protected override ValueTask TearDownAsync() => ValueTask.CompletedTask;
    }
}