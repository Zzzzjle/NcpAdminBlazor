using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.UsersManagement;

public sealed record GetUserIdByNameQuery(string Username) : IQuery<UserId?>;

public sealed class GetUserIdByNameQueryHandler(
    ApplicationDbContext dbContext)
    : IQueryHandler<GetUserIdByNameQuery, UserId?>
{
    public async Task<UserId?> Handle(GetUserIdByNameQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Where(u => u.Username == request.Username)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}