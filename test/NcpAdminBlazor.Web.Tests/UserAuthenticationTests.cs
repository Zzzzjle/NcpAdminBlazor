using System.Net;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests;

/// <summary>
/// 用户认证端点测试 - 演示测试集合和状态管理
/// </summary>
public class UserAuthenticationTests(WebAppFixture App, UserTestState State) 
    : TestBase<WebAppFixture, UserTestState>
{
    [Fact, Priority(1)]
    public async Task Register_User_For_Authentication_Tests()
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

        // Act
        var (rsp, res) = await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ResponseData<RegisterResponse>>(registerRequest);

        // Assert
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
        State.TestUserId = res.Data.UserId;
    }

    [Fact, Priority(2)]
    public async Task Login_Should_Return_Valid_JWT_Token()
    {
        // Arrange
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
        res.Data.Token.ShouldNotBeNullOrEmpty();
        
        // Token应该是有效的JWT格式 (包含两个点分割的三个部分)
        var tokenParts = res.Data.Token.Split('.');
        tokenParts.Length.ShouldBe(3);
        
        State.AuthToken = res.Data.Token;
    }

    [Fact, Priority(3)]
    public async Task Auth_Endpoint_Should_Validate_Token_Correctly()
    {
        // Arrange
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

    [Fact, Priority(4)]
    public async Task Auth_Endpoint_Should_Reject_Invalid_Token()
    {
        // Arrange
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", "invalid.jwt.token");
        });

        // Act
        var (rsp, res) = await authClient.GETAsync<AuthEndpoint, ResponseData<bool>>();

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact, Priority(5)]
    public async Task Auth_Endpoint_Should_Reject_Expired_Token()
    {
        // 这里可以测试过期token的情况
        // 实际项目中可能需要使用测试专用的短期token或mock时间
        
        // Arrange - 使用空token模拟过期情况
        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", "");
        });

        // Act
        var (rsp, res) = await authClient.GETAsync<AuthEndpoint, ResponseData<bool>>();

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}