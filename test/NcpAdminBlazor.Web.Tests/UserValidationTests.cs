using System.Net;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests;

/// <summary>
/// 用户输入验证测试 - 测试各种无效输入和边界条件
/// </summary>
public class UserValidationTests(WebAppFixture App, UserTestState State) 
    : TestBase<WebAppFixture, UserTestState>
{
    [Theory]
    [InlineData("", "valid@example.com", "Password123", "13800138000", "真实姓名")] // 空用户名
    [InlineData("a", "valid@example.com", "Password123", "13800138000", "真实姓名")] // 用户名太短
    [InlineData("verylongusernameverylongusernameverylongusernameverylongusername", "valid@example.com", "Password123", "13800138000", "真实姓名")] // 用户名太长
    public async Task RegisterEndpoint_Should_Validate_UserName(string name, string email, string password, string phone, string realName)
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Name = name,
            Email = email,
            Password = password,
            Phone = phone,
            RealName = realName
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorResponse>(registerRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.ShouldContainKey("Name");
    }

    [Theory]
    [InlineData("testuser", "", "Password123", "13800138000", "真实姓名")] // 空邮箱
    [InlineData("testuser", "invalid-email", "Password123", "13800138000", "真实姓名")] // 无效邮箱格式
    [InlineData("testuser", "no-at-symbol.com", "Password123", "13800138000", "真实姓名")] // 缺少@符号
    [InlineData("testuser", "@invalid.com", "Password123", "13800138000", "真实姓名")] // 缺少用户名部分
    [InlineData("testuser", "user@", "Password123", "13800138000", "真实姓名")] // 缺少域名部分
    public async Task RegisterEndpoint_Should_Validate_Email(string name, string email, string password, string phone, string realName)
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Name = name,
            Email = email,
            Password = password,
            Phone = phone,
            RealName = realName
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorResponse>(registerRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.ShouldContainKey("Email");
    }

    [Theory]
    [InlineData("testuser", "valid@example.com", "", "13800138000", "真实姓名")] // 空密码
    [InlineData("testuser", "valid@example.com", "123", "13800138000", "真实姓名")] // 密码太短
    [InlineData("testuser", "valid@example.com", "simplepassword", "13800138000", "真实姓名")] // 密码太简单
    public async Task RegisterEndpoint_Should_Validate_Password(string name, string email, string password, string phone, string realName)
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Name = name,
            Email = email,
            Password = password,
            Phone = phone,
            RealName = realName
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorResponse>(registerRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.ShouldContainKey("Password");
    }

    [Theory]
    [InlineData("testuser", "valid@example.com", "Password123", "", "真实姓名")] // 空手机号
    [InlineData("testuser", "valid@example.com", "Password123", "123", "真实姓名")] // 手机号太短
    [InlineData("testuser", "valid@example.com", "Password123", "abcdefghijk", "真实姓名")] // 手机号包含字母
    [InlineData("testuser", "valid@example.com", "Password123", "12345678901234567890", "真实姓名")] // 手机号太长
    public async Task RegisterEndpoint_Should_Validate_Phone(string name, string email, string password, string phone, string realName)
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Name = name,
            Email = email,
            Password = password,
            Phone = phone,
            RealName = realName
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorResponse>(registerRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.ShouldContainKey("Phone");
    }

    [Fact]
    public async Task LoginEndpoint_Should_Validate_Empty_LoginName()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            LoginName = "", // 空的登录名
            Password = "Password123"
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<LoginEndpoint, LoginRequest, ErrorResponse>(loginRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.ShouldContainKey("LoginName");
    }

    [Fact]
    public async Task LoginEndpoint_Should_Validate_Empty_Password()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            LoginName = "valid@example.com",
            Password = "" // 空密码
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<LoginEndpoint, LoginRequest, ErrorResponse>(loginRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.ShouldContainKey("Password");
    }

    [Fact]
    public async Task ChangePasswordEndpoint_Should_Validate_EmptyUserId()
    {
        // Arrange
        State.AuthToken.ShouldNotBeNullOrEmpty("需要先运行认证测试获取token");
        
        var changePasswordRequest = new ChangePasswordRequest
        {
            UserId = "", // 空用户ID
            OldPassword = "OldPassword123",
            NewPassword = "NewPassword123"
        };

        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", State.AuthToken);
        });

        // Act
        var (rsp, res) = await authClient.POSTAsync<ChangePasswordEndpoint, ChangePasswordRequest, ErrorResponse>(changePasswordRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.ShouldContainKey("UserId");
    }

    [Theory]
    [InlineData("", "NewPassword123")] // 空旧密码
    [InlineData("OldPassword123", "")] // 空新密码
    [InlineData("OldPassword123", "123")] // 新密码太短
    public async Task ChangePasswordEndpoint_Should_Validate_Passwords(string oldPassword, string newPassword)
    {
        // Arrange
        State.AuthToken.ShouldNotBeNullOrEmpty("需要先运行认证测试获取token");
        State.TestUserId.ShouldNotBeNullOrEmpty("需要先运行注册测试获取用户ID");
        
        var changePasswordRequest = new ChangePasswordRequest
        {
            UserId = State.TestUserId,
            OldPassword = oldPassword,
            NewPassword = newPassword
        };

        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", State.AuthToken);
        });

        // Act
        var (rsp, res) = await authClient.POSTAsync<ChangePasswordEndpoint, ChangePasswordRequest, ErrorResponse>(changePasswordRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        
        if (string.IsNullOrEmpty(oldPassword))
            res.Errors.ShouldContainKey("OldPassword");
        if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            res.Errors.ShouldContainKey("NewPassword");
    }
}