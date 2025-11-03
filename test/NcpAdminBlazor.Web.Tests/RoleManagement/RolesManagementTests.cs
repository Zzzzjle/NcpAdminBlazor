using System.Net;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Web.Application.Queries.RolesManagement;
using NcpAdminBlazor.Web.Endpoints.RolesManagement;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests.RoleManagement;

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

    [Fact, Priority(5)]
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