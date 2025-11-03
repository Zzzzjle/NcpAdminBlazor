using FastEndpoints;
using FastEndpoints.Security;
using NcpAdminBlazor.Web.AspNetCore;

namespace NcpAdminBlazor.Web.Endpoints.UsersManagement;

public class RefreshEndpoint : Endpoint<TokenRequest, ResponseData<MyTokenResponse>>
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
        var envelope = await svc.CreateCustomToken<ResponseData<MyTokenResponse>>(
            userId: req.UserId,
            privileges: _ => { },
            map: tr => tr.AsResponseData(),
            isRenewal: true,
            request: req
        );

        await Send.OkAsync(envelope, ct);
    }
}