using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.Users;

public sealed record GetUserIdsByRoleIdQuery(RoleId RoleId) : IQuery<List<ApplicationUserId>>;

public sealed class GetUserIdsByRoleIdQueryHandler(ApplicationDbContext dbContext)
    : IQueryHandler<GetUserIdsByRoleIdQuery, List<ApplicationUserId>>
{
    public async Task<List<ApplicationUserId>> Handle(GetUserIdsByRoleIdQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.ApplicationUsers
            .Where(u => u.Roles.Any(ur => ur.RoleId == request.RoleId))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);
    }
}