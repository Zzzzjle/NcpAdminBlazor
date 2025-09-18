using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Shared.EndpointsDtos.UserEndpoints;
using NcpAdminBlazor.Web.Application.Queries;
using UserRoleDto = NcpAdminBlazor.Shared.EndpointsDtos.UserEndpoints.UserRoleDto;

namespace NcpAdminBlazor.Web.Endpoints.UserEndpoints;

[Tags("Users")]
[HttpGet("/api/user/{UserId}")]
[Authorize]
public class GetUserInfoEndpoint(IMediator mediator) : Endpoint<GetUserInfoRequest, ResponseData<UserInfoResponse>>
{
    public override async Task HandleAsync(GetUserInfoRequest req, CancellationToken ct)
    {
        var userId = new ApplicationUserId(long.Parse(req.UserId));
        var query = new GetUserInfoQuery(userId);
        var userInfo = await mediator.Send(query, ct);
        
        // 转换为共享模型
        var response = new UserInfoResponse
        {
            Id = userInfo.Id.ToString(),
            Name = userInfo.Name,
            Email = userInfo.Email,
            Phone = userInfo.Phone,
            RealName = userInfo.RealName,
            FirstName = string.Empty, // 这些字段可能需要从 RealName 分解或设为空
            LastName = string.Empty,
            JobTitle = string.Empty,
            Status = userInfo.Status,
            CreatedAt = userInfo.CreatedAt,
            Roles = userInfo.Roles.Select(r => new UserRoleDto 
            { 
                RoleName = r.RoleName, 
                RoleId = r.RoleId 
            }).ToList(),
            Permissions = userInfo.Permissions
        };
        
        await Send.OkAsync(response.AsResponseData(), ct);
    }
}