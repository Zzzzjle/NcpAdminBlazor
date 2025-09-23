using System.Net;
using System.Net.Http.Headers;
using FastEndpoints.Security;
using NcpAdminBlazor.Web.Endpoints.Users;
using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests;

[Collection(WebAppTestCollection.Name)]
public class UsersEndpointsTests(WebAppFixture app, UsersEndpointsTests.UserState state)
    : TestBase<WebAppFixture, UsersEndpointsTests.UserState>
{
    [Fact, Priority(1)]
    public async Task RegisterUser_ShouldReturn200_AndUserId()
    {
        // Act
        var (rsp, res) = await app.DefaultClient
            .POSTAsync<RegisterUserEndpoint, RegisterUserRequest, ResponseData<RegisterUserResponse>>(
                new RegisterUserRequest(UserState.Username, UserState.Password));
        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Data.UserId.Id.ShouldBeGreaterThan(0);
    }

    [Fact, Priority(2)]
    public async Task Login_ShouldReturn_TokenEnvelope()
    {
        // Act: 登录
        var (rsp, res) = await app.DefaultClient
            .POSTAsync<LoginEndpoint, LoginRequest, ResponseData<TokenResponse>>(
                new LoginRequest(UserState.Username, UserState.Password));

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.UserId.ShouldNotBeNullOrEmpty();
        res.Data.AccessToken.ShouldNotBeNullOrWhiteSpace();
        res.Data.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        state.Token = res.Data;
    }

    [Fact, Priority(3)]
    public async Task RefreshToken_ShouldIssue_NewAccessToken()
    {
        var token = state.Token ??
                    throw new InvalidOperationException(
                        "Token is null, ensure that Login_ShouldReturn_TokenEnvelope runs before this test.");
        // Act: 刷新令牌（端点允许匿名访问）
        var (rsp, res) = await app.DefaultClient
            .POSTAsync<RefreshEndpoint, TokenRequest, ResponseData<TokenResponse>>(
                new TokenRequest { UserId = token.UserId, RefreshToken = token.RefreshToken });

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.AccessToken.ShouldNotBeNullOrWhiteSpace();
        res.Data.RefreshToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact, Priority(4)]
    public async Task Profile_ShouldReturn_CurrentUserPayload()
    {
        // Arrange
        var token = state.Token?.AccessToken ?? throw new InvalidOperationException(
            "Token is null, ensure that Login_ShouldReturn_TokenEnvelope runs before this test.");

        app.DefaultClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        try
        {
            // Act
            var (rsp, res) = await app.DefaultClient.GETAsync<ProfileEndpoint, object>();

            // Assert
            rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
            res.ShouldNotBeNull();
        }
        finally
        {
            app.DefaultClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    /// <summary>
    /// use for sharing state between UserEndpointTests
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class UserState : StateFixture
    {
        public const string Username = "tesUser_";
        public const string Password = "Test@1234";

        public TokenResponse? Token;

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