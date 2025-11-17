using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.ApiService.Endpoints.RolesManagement;

namespace NcpAdminBlazor.ApiService.Application.Queries.RolesManagement;

public record GetRoleInfoQuery(RoleId RoleId) : IQuery<RoleInfoResponse>;

public class GetRoleInfoQueryValidator : AbstractValidator<GetRoleInfoQuery>
{
    public GetRoleInfoQueryValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}

public class GetRoleInfoQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetRoleInfoQuery, RoleInfoResponse>
{
    public async Task<RoleInfoResponse> Handle(GetRoleInfoQuery request, CancellationToken cancellationToken)
    {
        var roleInfo = await context.Roles
            .Where(r => r.Id == request.RoleId)
            .Select(r => new RoleInfoResponse(r.Id, r.Name, r.Description, r.IsDisabled))
            .FirstOrDefaultAsync(cancellationToken);

        return roleInfo ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");
    }
}