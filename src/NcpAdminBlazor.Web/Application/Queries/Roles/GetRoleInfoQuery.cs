using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.Roles;

public record GetRoleInfoQuery(RoleId RoleId) : IQuery<RoleInfoDto>;

public record RoleInfoDto(
    RoleId RoleId,
    string Name,
    string Description,
    int Status,
    DateTimeOffset CreatedAt);

public class GetRoleInfoQueryValidator : AbstractValidator<GetRoleInfoQuery>
{
    public GetRoleInfoQueryValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}

public class GetRoleInfoQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetRoleInfoQuery, RoleInfoDto>
{
    public async Task<RoleInfoDto> Handle(GetRoleInfoQuery request, CancellationToken cancellationToken)
    {
        var roleInfo = await context.Roles
            .Where(r => r.Id == request.RoleId)
            .Select(r => new RoleInfoDto(r.Id, r.Name, r.Description, r.Status, r.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return roleInfo ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");
    }
}
