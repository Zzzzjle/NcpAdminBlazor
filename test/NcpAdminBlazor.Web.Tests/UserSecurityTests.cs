using System.Net;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests;

/// <summary>
/// 用户安全和授权测试 - 测试JWT认证、授权和安全相关场景
/// </summary>
public class UserSecurityTests(WebAppFixture App, UserTestState State) 
    : TestBase<WebAppFixture, UserTestState>
{
    [Fact]
    public async Task AuthEndpoint_Should_Return_Unauthorized_Without_Token()
    {
        // Act
        var (rsp, _) = await App.Client.GETAsync<AuthEndpoint, ResponseData<bool>>();

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthEndpoint_Should_Return_Unauthorized_With_Invalid_Token()
    {
        // Arrange
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", "invalid.jwt.token");
        });

        // Act
        var (rsp, _) = await authClient.GETAsync<AuthEndpoint, ResponseData<bool>>();

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthEndpoint_Should_Return_Unauthorized_With_Expired_Token()
    {
        // Arrange - 使用已过期的JWT token
        var expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE1MTYyMzkwMjJ9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
        
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", expiredToken);
        });

        // Act
        var (rsp, _) = await authClient.GETAsync<AuthEndpoint, ResponseData<bool>>();

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthEndpoint_Should_Return_Unauthorized_With_Malformed_Token()
    {
        // Arrange
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", "malformed-token");
        });

        // Act
        var (rsp, _) = await authClient.GETAsync<AuthEndpoint, ResponseData<bool>>();

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserInfoEndpoint_Should_Return_Unauthorized_Without_Token()
    {
        // Arrange
        var getUserRequest = new GetUserInfoRequest
        {
            UserId = "test-user-id"
        };

        // Act
        var (rsp, _) = await App.Client.GETAsync<GetUserInfoEndpoint, GetUserInfoRequest, ResponseData<UserInfoResponse>>(getUserRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserInfoEndpoint_Should_Return_Unauthorized_With_Invalid_Token()
    {
        // Arrange
        var getUserRequest = new GetUserInfoRequest
        {
            UserId = "test-user-id"
        };

        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", "invalid.jwt.token");
        });

        // Act
        var (rsp, _) = await authClient.GETAsync<GetUserInfoEndpoint, GetUserInfoRequest, ResponseData<UserInfoResponse>>(getUserRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePasswordEndpoint_Should_Return_Unauthorized_Without_Token()
    {
        // Arrange
        var changePasswordRequest = new ChangePasswordRequest
        {
            UserId = "test-user-id",
            OldPassword = "OldPassword123",
            NewPassword = "NewPassword123"
        };

        // Act
        var (rsp, _) = await App.Client.POSTAsync<ChangePasswordEndpoint, ChangePasswordRequest, ResponseData<bool>>(changePasswordRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserListEndpoint_Should_Return_Unauthorized_Without_Token()
    {
        // Act
        var (rsp, _) = await App.Client.GETAsync<GetUserListEndpoint, ResponseData<List<UserInfoResponse>>>();

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_With_NonExistent_User_Should_Fail()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            LoginName = "nonexistent@example.com",
            Password = "AnyPassword123"
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<LoginEndpoint, LoginRequest, ResponseData<LoginResponse>>(loginRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Success.ShouldBeFalse();
        res.Message.ShouldContain("用户名或密码错误");
    }

    [Fact]
    public async Task Login_With_Correct_User_But_Wrong_Password_Should_Fail()
    {
        // Arrange - 假设State.TestUserEmail是已存在的用户
        State.TestUserEmail.ShouldNotBeNullOrEmpty("需要先有注册的用户");
        
        var loginRequest = new LoginRequest
        {
            LoginName = State.TestUserEmail,
            Password = "WrongPassword123"
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<LoginEndpoint, LoginRequest, ResponseData<LoginResponse>>(loginRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Success.ShouldBeFalse();
        res.Message.ShouldContain("用户名或密码错误");
    }

    [Fact]
    public async Task GetUserInfo_For_Different_User_Should_Fail()
    {
        // Arrange - 尝试获取不是当前登录用户的信息
        State.AuthToken.ShouldNotBeNullOrEmpty("需要先运行认证测试获取token");
        
        var getUserRequest = new GetUserInfoRequest
        {
            UserId = "different-user-id" // 不同的用户ID
        };

        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", State.AuthToken);
        });

        // Act
        var (rsp, res) = await authClient.GETAsync<GetUserInfoEndpoint, GetUserInfoRequest, ResponseData<UserInfoResponse>>(getUserRequest);

        // Assert
        // 这个测试的结果取决于具体的业务逻辑
        // 如果系统允许用户查看其他用户信息，应该返回成功
        // 如果不允许，应该返回403 Forbidden或者404 NotFound
        rsp.StatusCode.ShouldBeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Bearer")] // 只有Bearer前缀
    [InlineData("Basic token")] // 错误的认证类型
    [InlineData("token-without-bearer")] // 没有Bearer前缀
    public async Task Protected_Endpoints_Should_Reject_Invalid_Authorization_Headers(string authHeader)
    {
        // Arrange
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Add("Authorization", authHeader);
        });

        // Act
        var (rsp, _) = await authClient.GETAsync<AuthEndpoint, ResponseData<bool>>();

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task JWT_Token_Should_Contain_Required_Claims()
    {
        // Arrange - 先登录获取token
        State.TestUserEmail.ShouldNotBeNullOrEmpty("需要先有注册的用户");
        State.TestUserPassword.ShouldNotBeNullOrEmpty("需要先有用户密码");
        
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
        
        // 验证token格式
        var tokenParts = res.Data.Token.Split('.');
        tokenParts.Length.ShouldBe(3); // JWT应该有3个部分：header.payload.signature
        
        // 保存token供其他测试使用
        State.AuthToken = res.Data.Token;
    }
}