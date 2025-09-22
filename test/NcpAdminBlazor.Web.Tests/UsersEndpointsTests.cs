using System.Net;
using NcpAdminBlazor.Web.Endpoints.Users;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests;

[Collection(WebAppTestCollection.Name)]
public class UsersEndpointsTests(WebAppFixture app) : TestBase<WebAppFixture>
{
    [Fact]
    public async Task RegisterUser_ShouldReturn200_AndUserId()
    {
        // Act
        var (rsp, res) =
            await app.DefaultClient
                .POSTAsync<RegisterUserEndpoint, RegisterUserRequest, ResponseData<RegisterUserResponse>>(
                    new RegisterUserRequest($"user_{Guid.NewGuid():N}"));
        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Data.UserId.Id.ShouldBeGreaterThan(0);
    }
}