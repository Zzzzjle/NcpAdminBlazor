using Microsoft.AspNetCore.Identity.Data;
using NcpAdminBlazor.Web.Endpoints.Users;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests;

/// <summary>
/// 用户认证端点测试 - 演示测试集合和状态管理
/// </summary>
public class UserAuthenticationTests(WebAppFixture app)
    : TestBase<WebAppFixture>
{
    [Fact, Priority(1)]
    public async Task Register_User_For_Authentication_Tests()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest("testUser");

        // Act
        var (rsp, res) =
            await app.Client.POSTAsync<RegisterUserEndpoint, RegisterUserRequest, ResponseData<RegisterUserResponse>>(
                registerRequest);

        // Assert
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
    }
}