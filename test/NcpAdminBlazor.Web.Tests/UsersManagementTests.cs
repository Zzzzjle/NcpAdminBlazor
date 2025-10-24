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

    [Fact, Priority(4)]
    public async Task CreateUserEndpoint_ShouldReturnSuccess_AndPersistUser()
    {
        // Arrange
        var uniqueSuffix = Guid.NewGuid().ToString("N")[..8];
        var username = $"create_user_{uniqueSuffix}";
        var email = $"{username}@example.com";
        var phone = $"1{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds():0000000000000}"[..11];
        var request = new CreateUserRequest
        {
            Username = username,
            Password = "Test@1234!",
            RealName = "Auto User",
            Email = email,
            Phone = phone,
            Status = 1
        };

        // Act
        var (rsp, res) = await app.AuthenticatedClient
            .POSTAsync<CreateUserEndpoint, CreateUserRequest, ResponseData<CreateUserResponse>>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.UserId.Id.ShouldBeGreaterThan(0);

        state.CreatedUserId = res.Data.UserId;
        state.CreatedUsername = username;

        var (_, listResponse) = await app.AuthenticatedClient
            .GETAsync<UserListEndpoint, GetUserListRequest, ResponseData<PagedData<UserListItemDto>>>(
                new GetUserListRequest { Username = username, PageSize = 1 });

        listResponse.Success.ShouldBeTrue();
        listResponse.Data.Items.ShouldContain(u => u.Username == username);
    }

    [Fact, Priority(5)]
    public async Task UpdateUserEndpoint_ShouldUpdateExistingUser()
    {
        // Arrange
        var userId = state.CreatedUserId ?? throw new InvalidOperationException("Created user not initialized");
        var username = state.CreatedUsername ?? throw new InvalidOperationException("Created username not initialized");
        var updatedRealName = "Updated Auto User";
        var updatedEmail = $"{username}+updated@example.com";
        var updatedPhone = $"1{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds():0000000000000}"[..11];
        var request = new UpdateUserRequest
        {
            UserId = userId,
            Username = username,
            RealName = updatedRealName,
            Email = updatedEmail,
            Phone = updatedPhone,
            Status = 0
        };

        // Act
        var (rsp, res) = await app.AuthenticatedClient
            .POSTAsync<UpdateUserEndpoint, UpdateUserRequest, ResponseData>(request);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();

        var (_, listResponse) = await app.AuthenticatedClient
            .GETAsync<UserListEndpoint, GetUserListRequest, ResponseData<PagedData<UserListItemDto>>>(
                new GetUserListRequest { Username = username, PageSize = 1 });

        listResponse.Success.ShouldBeTrue();
        listResponse.Data.Items.ShouldContain(user =>
            user.Username == username &&
            user.RealName == updatedRealName &&
            user.Email == updatedEmail &&
            user.Phone == updatedPhone &&
            user.Status == 0);
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
        public ApplicationUserId? CreatedUserId;
        public string? CreatedUsername;

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