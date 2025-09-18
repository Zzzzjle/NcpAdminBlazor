using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Web.Application.Queries;

public record CheckRefreshTokenIsValidQuery(ApplicationUserId UserId, string RefreshToken)
    : IQuery<bool>;

public class CheckRefreshTokenIsValidQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckRefreshTokenIsValidQuery, bool>
{
    public async Task<bool> Handle(CheckRefreshTokenIsValidQuery request, CancellationToken cancellationToken)
    {
        return await context.ApplicationUsers.AnyAsync(user => user.Id == request.UserId &&
                                                               user.RefreshToken == request.RefreshToken &&
                                                               user.RefreshExpiry >= DateTime.UtcNow,
            cancellationToken: cancellationToken);
    }
}