using FastEndpoints;
using FastEndpoints.Security;

namespace NcpAdminBlazor.Web.Endpoints.Users;

public class RefreshEndpoint : Endpoint<TokenRequest, ResponseData<TokenResponse>>
{
    public override void Configure()
    {
        Post("/api/user/refresh-token");
        Description(x => x.WithTags("User"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(TokenRequest req, CancellationToken ct)
    {
        var svc = Resolve<UserTokenService>();
        var envelope = await svc.CreateCustomToken<ResponseData<TokenResponse>>(
            userId: req.UserId,
            privileges: _ => { },
            map: tr => tr.AsResponseData(),
            isRenewal: true,
            request: req
        );

        await Send.OkAsync(envelope, ct);
    }
}