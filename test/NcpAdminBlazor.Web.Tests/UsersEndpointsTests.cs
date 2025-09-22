using System.Net;
using FastEndpoints.Security;
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
        var (rsp, res) = await app.DefaultClient
            .POSTAsync<RegisterUserEndpoint, RegisterUserRequest, ResponseData<RegisterUserResponse>>(
                new RegisterUserRequest($"user_{Guid.NewGuid():N}"));
        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Data.UserId.Id.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Login_ShouldReturn_TokenEnvelope()
    {
        // Arrange: 先注册一个可以登录的用户（端点内部使用固定密码 1231231）
        var userName = $"user_{Guid.NewGuid():N}";
        var (crsp, _) = await app.DefaultClient
            .POSTAsync<RegisterUserEndpoint, RegisterUserRequest, ResponseData<RegisterUserResponse>>(
                new RegisterUserRequest(userName));
        crsp.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Act: 登录
        var (lrsp, lres) = await app.DefaultClient
            .POSTAsync<LoginEndpoint, LoginRequest, ResponseData<TokenResponse>>(
                new LoginRequest(userName, "1231231"));

        // Assert
        lrsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        lres.Success.ShouldBeTrue();
        lres.Data.AccessToken.ShouldNotBeNullOrWhiteSpace();
        lres.Data.RefreshToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task RefreshToken_ShouldIssue_NewAccessToken()
    {
        // Arrange: 注册并登录，拿到 refreshToken
        var userName = $"user_{Guid.NewGuid():N}";
        var (crsp, _) = await app.DefaultClient
            .POSTAsync<RegisterUserEndpoint, RegisterUserRequest, ResponseData<RegisterUserResponse>>(
                new RegisterUserRequest(userName));
        crsp.StatusCode.ShouldBe(HttpStatusCode.OK);

        var (_, loginRes) = await app.DefaultClient
            .POSTAsync<LoginEndpoint, LoginRequest, ResponseData<TokenResponse>>(
                new LoginRequest(userName, "1231231"));

        var token = loginRes.Data;

        // Act: 刷新令牌（端点允许匿名访问）
        var (rrsp, rres) = await app.DefaultClient
            .POSTAsync<RefreshEndpoint, TokenRequest, ResponseData<TokenResponse>>(
                new TokenRequest { UserId = token.UserId, RefreshToken = token.RefreshToken });

        // Assert
        rrsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        rres.Success.ShouldBeTrue();
        rres.Data.AccessToken.ShouldNotBeNullOrWhiteSpace();
        rres.Data.RefreshToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Profile_ShouldReturn_CurrentUserPayload()
    {
        // Act
        var (rsp, res) = await app.AuthenticatedClient
            .GETAsync<ProfileEndpoint, object>();

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
    }
}