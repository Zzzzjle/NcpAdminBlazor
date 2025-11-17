using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.UsersManagement;

public sealed record GetUserIdsByRoleIdQuery(RoleId RoleId) : IQuery<List<UserId>>;

public sealed class GetUserIdsByRoleIdQueryHandler(ApplicationDbContext dbContext)
    : IQueryHandler<GetUserIdsByRoleIdQuery, List<UserId>>
{
    public async Task<List<UserId>> Handle(GetUserIdsByRoleIdQuery request,
        CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .Where(u => !u.IsDeleted)
            .Select(u => new { u.Id, u.AssignedRoleIds })
            .ToListAsync(cancellationToken);

        return users
            .Where(u => u.AssignedRoleIds.Contains(request.RoleId))
            .Select(u => u.Id)
            .ToList();
    }
}