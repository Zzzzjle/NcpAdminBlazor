using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.Users;

public record CheckUserExistsByUsernameExceptIdQuery(string Username, ApplicationUserId UserId) : IQuery<bool>;

public class CheckUserExistsByUsernameExceptIdQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckUserExistsByUsernameExceptIdQuery, bool>
{
    public async Task<bool> Handle(CheckUserExistsByUsernameExceptIdQuery request, CancellationToken cancellationToken)
    {
        return await context.ApplicationUsers
            .AnyAsync(u => u.Username == request.Username && u.Id != request.UserId, cancellationToken);
    }
}