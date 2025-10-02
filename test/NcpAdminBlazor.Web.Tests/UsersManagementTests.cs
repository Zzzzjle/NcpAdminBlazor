using System.Net;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Web.Application.Queries;
using NcpAdminBlazor.Web.Endpoints.Users;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests;

[Collection(AuthenticatedTestCollection.Name)]
public class UsersManagementTests(AuthenticatedAppFixture app, UsersManagementTests.UserState state)
    : TestBase<AuthenticatedAppFixture, UsersManagementTests.UserState>
{
    [Fact, Priority(1)]
    public async Task RegisterUser_ShouldReturn200_AndUserId()
    {
        // Act
        var (rsp, res) = await app.Client
            .POSTAsync<RegisterUserEndpoint, RegisterUserRequest, ResponseData<RegisterUserResponse>>(
                new RegisterUserRequest(UserState.Username, UserState.Password));
        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Data.UserId.Id.ShouldBeGreaterThan(0);
        state.RegisteredUserId = res.Data.UserId;
    }


    [Fact, Priority(2)]
    public async Task UserList_ShouldContain_NewUser()
    {
        var (rsp, res) = await app.AuthenticatedClient
            .GETAsync<UserListEndpoint, GetUserListRequest, ResponseData<PagedData<UserListItemDto>>>(
                new GetUserListRequest { Username = UserState.Username, PageSize = 20 });

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.Items.ShouldContain(u => u.Username == UserState.Username);
    }

    [Fact, Priority(3)]
    public async Task DeleteUser_ShouldSoftDelete_User()
    {
        var userId = state.RegisteredUserId ?? throw new InvalidOperationException("UserId not initialized");

        var (rsp, res) = await app.AuthenticatedClient
            .DELETEAsync<DeleteUserEndpoint, DeleteUserRequest, ResponseData>(new DeleteUserRequest
                { UserId = userId.Id });

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();

        // Verify user no longer appears in list
        var (_, listResponse) = await app.Client
            .GETAsync<UserListEndpoint, GetUserListRequest, ResponseData<PagedData<UserListItemDto>>>(
                new GetUserListRequest { Username = UserState.Username, PageSize = 20 });

        listResponse.Data.Items.ShouldNotContain(u => u.Username == UserState.Username);
    }

    /// <summary>
    /// use for sharing state between UsersManagementTests
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class UserState : StateFixture
    {
        public const string Username = "tesUser_2";
        public const string Password = "Test@1234";

        public ApplicationUserId? RegisteredUserId;

        protected override async ValueTask SetupAsync()
        {
            await ValueTask.CompletedTask;
        }

        protected override async ValueTask TearDownAsync()
        {
            await ValueTask.CompletedTask;
        }
    }
}