using System.Net;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests;

/// <summary>
/// 用户信息管理端点测试 - 使用FastEndpoints测试框架
/// </summary>
public class UserInfoTests(WebAppFixture App, UserTestState State) 
    : TestBase<WebAppFixture, UserTestState>
{
    [Fact, Priority(1)]
    public async Task GetUserInfo_Without_Auth_Should_Return_Unauthorized()
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

    [Fact, Priority(2)]
    public async Task GetUserInfo_With_Valid_Token_Should_Return_UserInfo()
    {
        // Arrange - 确保有有效的token和用户ID
        State.AuthToken.ShouldNotBeNullOrEmpty("需要先运行认证测试获取token");
        State.TestUserId.ShouldNotBeNullOrEmpty("需要先运行注册测试获取用户ID");
        
        var getUserRequest = new GetUserInfoRequest
        {
            UserId = State.TestUserId
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
        res.Data.Name.ShouldBe(State.TestUserName);
        res.Data.Email.ShouldBe(State.TestUserEmail);
        res.Data.Phone.ShouldBe(State.TestUserPhone);
        res.Data.RealName.ShouldBe(State.TestUserRealName);
    }

    [Fact, Priority(3)]
    public async Task ChangePassword_Should_Update_Password_Successfully()
    {
        // Arrange
        State.AuthToken.ShouldNotBeNullOrEmpty("需要先运行认证测试获取token");
        State.TestUserId.ShouldNotBeNullOrEmpty("需要先运行注册测试获取用户ID");
        
        var newPassword = "NewPassword123!";
        var changePasswordRequest = new ChangePasswordRequest
        {
            UserId = State.TestUserId,
            OldPassword = State.TestUserPassword,
            NewPassword = newPassword
        };

        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", State.AuthToken);
        });

        // Act
        var (rsp, res) = await authClient.POSTAsync<ChangePasswordEndpoint, ChangePasswordRequest, ResponseData<bool>>(changePasswordRequest);

        // Assert
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
        res.Data.ShouldBeTrue();
        
        // 更新测试状态
        State.TestUserPassword = newPassword;
    }

    [Fact, Priority(4)]
    public async Task Login_With_New_Password_Should_Work()
    {
        // Arrange - 使用新密码登录
        var loginRequest = new LoginRequest
        {
            LoginName = State.TestUserEmail,
            Password = State.TestUserPassword // 使用更新后的密码
        };

        // Act
        var (rsp, res) = await App.Client.POSTAsync<LoginEndpoint, LoginRequest, ResponseData<LoginResponse>>(loginRequest);

        // Assert
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
        res.Data.Token.ShouldNotBeNullOrEmpty();
    }

    [Fact, Priority(5)]
    public async Task ChangePassword_With_Wrong_Current_Password_Should_Fail()
    {
        // Arrange
        State.AuthToken.ShouldNotBeNullOrEmpty("需要先运行认证测试获取token");
        State.TestUserId.ShouldNotBeNullOrEmpty("需要先运行注册测试获取用户ID");
        
        var changePasswordRequest = new ChangePasswordRequest
        {
            UserId = State.TestUserId,
            OldPassword = "WrongPassword123!",
            NewPassword = "AnotherNewPassword123!"
        };

        var authClient = App.CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new("Bearer", State.AuthToken);
        });

        // Act
        var (rsp, res) = await authClient.POSTAsync<ChangePasswordEndpoint, ChangePasswordRequest, ResponseData<bool>>(changePasswordRequest);

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Success.ShouldBeFalse();
    }
}