using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.Users;

public sealed record GetUserIdByNameQuery(string Username) : IQuery<ApplicationUserId?>;

public sealed class GetUserIdByNameQueryHandler(
    ApplicationDbContext dbContext)
    : IQueryHandler<GetUserIdByNameQuery, ApplicationUserId?>
{
    public async Task<ApplicationUserId?> Handle(GetUserIdByNameQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ApplicationUsers
            .Where(u => u.Username == request.Username)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}