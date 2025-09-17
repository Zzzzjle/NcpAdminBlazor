using System.Net;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests;

/// <summary>
/// 用户端点集成测试 - 使用FastEndpoints测试框架的无路由测试策略
/// </summary>
public class UserEndpointsIntegrationTests(WebAppFixture App, UserTestState State) 
    : TestBase<WebAppFixture, UserTestState>
{
    [Fact, Priority(1)]
    public async Task RegisterEndpoint_Should_Return_Success_When_Valid_Request()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Name = State.TestUserName,
            Email = State.TestUserEmail,
            Password = State.TestUserPassword,
            Phone = State.TestUserPhone,
            RealName = State.TestUserRealName
        };

        // Act - 使用FastEndpoints的无路由POST方法
        var (rsp, res) = await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ResponseData<RegisterResponse>>(registerRequest);

        // Assert - 使用Shouldly断言
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
        res.Data.ShouldNotBeNull();
        res.Data.UserId.ShouldNotBeNullOrEmpty();
        
        // 保存用户ID供后续测试使用
        State.TestUserId = res.Data.UserId;
    }

    [Fact, Priority(2)]
    public async Task RegisterEndpoint_Should_Return_BadRequest_When_Invalid_Email()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Name = "testuser",
            Email = "invalid-email", // 无效邮箱格式
            Password = "TestPassword123",
            Phone = "13800138001",
            RealName = "测试用户"
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorResponse>(registerRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.ShouldContainKey("Email");
    }

    [Fact, Priority(3)]
    public async Task RegisterEndpoint_Should_Return_BadRequest_When_Duplicate_Email()
    {
        // Arrange - 尝试用相同邮箱注册第二个用户
        var duplicateRequest = new RegisterRequest
        {
            Name = "seconduser" + Guid.NewGuid().ToString("N")[..8],
            Email = State.TestUserEmail, // 使用已存在的邮箱
            Password = "TestPassword123",
            Phone = "13800138002",
            RealName = "第二个用户"
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ResponseData<object>>(duplicateRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Success.ShouldBeFalse();
        res.Message.ShouldContain("邮箱已存在");
    }

    [Fact, Priority(4)]
    public async Task LoginEndpoint_Should_Return_Success_When_Valid_Credentials()
    {
        // Arrange - 使用已注册的用户凭据
        var loginRequest = new LoginRequest
        {
            LoginName = State.TestUserEmail,
            Password = State.TestUserPassword
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<LoginEndpoint, LoginRequest, ResponseData<LoginResponse>>(loginRequest);

        // Assert
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
        res.Data.ShouldNotBeNull();
        res.Data.Token.ShouldNotBeNullOrEmpty();
        res.Data.Name.ShouldBe(State.TestUserName);
        res.Data.Email.ShouldBe(State.TestUserEmail);
        res.Data.UserId.ShouldBe(State.TestUserId);
        
        // 保存token供后续测试使用
        State.AuthToken = res.Data.Token;
    }

    [Fact, Priority(5)]
    public async Task LoginEndpoint_Should_Return_BadRequest_When_Invalid_Credentials()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            LoginName = "nonexistent@example.com",
            Password = "WrongPassword"
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<LoginEndpoint, LoginRequest, ResponseData<object>>(loginRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Success.ShouldBeFalse();
    }

    [Fact, Priority(6)]
    public async Task AuthEndpoint_Should_Return_Unauthorized_Without_Token()
    {
        // Act
        var (rsp, res) = await App.Client.GETAsync<AuthEndpoint, ResponseData<bool>>();

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact, Priority(7)]
    public async Task AuthEndpoint_Should_Return_Success_With_Valid_Token()
    {
        // Arrange - 使用认证客户端
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", State.AuthToken);
        });

        // Act
        var (rsp, res) = await authClient.GETAsync<AuthEndpoint, ResponseData<bool>>();

        // Assert
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
        res.Data.ShouldBeTrue();
    }

    [Fact, Priority(8)]
    public async Task GetUserInfoEndpoint_Should_Return_Success_With_Valid_Token()
    {
        // Arrange - 创建带认证的请求
        var getUserRequest = new GetUserInfoRequest
        {
            UserId = State.TestUserId!
        };
        
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", State.AuthToken);
        });

        // Act
        var (rsp, res) = await authClient.GETAsync<GetUserInfoEndpoint, GetUserInfoRequest, ResponseData<UserInfoResponse>>(getUserRequest);

        // Assert
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
        res.Data.ShouldNotBeNull();
        res.Data.Id.ShouldBe(State.TestUserId);
        res.Data.Name.ShouldBe(State.TestUserName);
        res.Data.Email.ShouldBe(State.TestUserEmail);
    }

    [Fact, Priority(9)]
    public async Task GetUserInfoEndpoint_Should_Return_Unauthorized_Without_Token()
    {
        // Arrange
        var getUserRequest = new GetUserInfoRequest
        {
            UserId = State.TestUserId!
        };

        // Act
        var (rsp, res) = await App.Client.GETAsync<GetUserInfoEndpoint, GetUserInfoRequest, ResponseData<UserInfoResponse>>(getUserRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact, Priority(10)]
    public async Task ChangePasswordEndpoint_Should_Return_Success_When_Valid_Request()
    {
        // Arrange
        var changePasswordRequest = new ChangePasswordRequest
        {
            OldPassword = State.TestUserPassword,
            NewPassword = "NewTestPassword123"
        };
        
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", State.AuthToken);
        });

        // Act
        var (rsp, res) = await authClient.PUTAsync<ChangePasswordEndpoint, ChangePasswordRequest, ResponseData<object>>(changePasswordRequest);

        // Assert
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
    }

    [Fact, Priority(11)]
    public async Task ChangePasswordEndpoint_Should_Return_BadRequest_When_Wrong_Current_Password()
    {
        // Arrange
        var changePasswordRequest = new ChangePasswordRequest
        {
            OldPassword = "WrongCurrentPassword",
            NewPassword = "NewTestPassword123"
        };
        
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", State.AuthToken);
        });

        // Act
        var (rsp, res) = await authClient.PUTAsync<ChangePasswordEndpoint, ChangePasswordRequest, ResponseData<object>>(changePasswordRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Success.ShouldBeFalse();
    }

    [Fact, Priority(12)]
    public async Task GetUserListEndpoint_Should_Return_Success_With_Valid_Token()
    {
        // Arrange
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", State.AuthToken);
        });

        // Act
        var (rsp, res) = await authClient.GETAsync<GetUserListEndpoint, ResponseData<List<UserInfoResponse>>>();

        // Assert
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
        res.Data.ShouldNotBeNull();
        res.Data.ShouldNotBeEmpty();
        
        // 验证列表中包含我们创建的测试用户
        var testUser = res.Data.FirstOrDefault(u => u.Id == State.TestUserId);
        testUser.ShouldNotBeNull();
        testUser.Name.ShouldBe(State.TestUserName);
        testUser.Email.ShouldBe(State.TestUserEmail);
    }
}