using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.UsersManagement;

public record CheckRefreshTokenIsValidQuery(UserId UserId, string RefreshToken)
    : IQuery<bool>;

public class CheckRefreshTokenIsValidQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckRefreshTokenIsValidQuery, bool>
{
    public async Task<bool> Handle(CheckRefreshTokenIsValidQuery request, CancellationToken cancellationToken)
    {
    return await context.Users.AnyAsync(user => user.Id == request.UserId &&
                           user.RefreshToken == request.RefreshToken &&
                           user.RefreshExpiry >= DateTime.UtcNow,
            cancellationToken: cancellationToken);
    }
}