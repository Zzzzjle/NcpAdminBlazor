using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.Users;

public record GetUsernameByIdQuery(ApplicationUserId UserId) : IQuery<string>;

public class GetUsernameByIdQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetUsernameByIdQuery, string>
{
    public async Task<string> Handle(GetUsernameByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.ApplicationUsers
                   .Where(u => u.Id == request.UserId)
                   .Select(u => u.Username ?? string.Empty)
                   .FirstOrDefaultAsync(cancellationToken)
               ?? throw new KnownException("User not found");
    }
}