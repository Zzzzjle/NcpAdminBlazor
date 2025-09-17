using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NcpAdminBlazor.Web.Application.Queries;

namespace NcpAdminBlazor.Web.Endpoints.UserEndpoints;

public record GetUserListRequest(string? Name, string? Email, int? Status, int PageIndex = 1, int PageSize = 10);

[Tags("Users")]
[HttpGet("/api/users")]
[Authorize]
public class GetUserListEndpoint(IMediator mediator) : Endpoint<GetUserListRequest, ResponseData<PagedData<UserListItemDto>>>
{
    public override async Task HandleAsync(GetUserListRequest req, CancellationToken ct)
    {
        var query = new GetUserListQuery(req.Name, req.Email, req.Status, req.PageIndex, req.PageSize);
        var userList = await mediator.Send(query, ct);
        
        await Send.OkAsync(userList.AsResponseData(), ct);
    }
}