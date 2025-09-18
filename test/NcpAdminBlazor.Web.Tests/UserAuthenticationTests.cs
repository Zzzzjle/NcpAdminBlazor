using NcpAdminBlazor.Shared.EndpointsDtos.UserEndpoints;
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
        var registerRequest = new RegisterRequest
        {
            Name = "testUser",
            Email = "testUser@qq.com",
            Password = "testUser123T",
            Phone = "13348347765",
            RealName = "testUser",
        };

        // Act
        var (rsp, res) = await app.Client.POSTAsync<RegisterEndpoint, RegisterRequest, ResponseData<RegisterResponse>>(registerRequest);

        // Assert
        rsp.IsSuccessStatusCode.ShouldBeTrue();
        res.Success.ShouldBeTrue();
    }
    
}