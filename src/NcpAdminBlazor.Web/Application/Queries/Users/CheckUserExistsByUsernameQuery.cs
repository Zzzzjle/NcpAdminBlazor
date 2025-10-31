using Microsoft.EntityFrameworkCore;

namespace NcpAdminBlazor.Web.Application.Queries.Users;

public record CheckUserExistsByUsernameQuery(string Username) : IQuery<bool>;

public class CheckUserExistsByUsernameQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckUserExistsByUsernameQuery, bool>
{
    public async Task<bool> Handle(CheckUserExistsByUsernameQuery request, CancellationToken cancellationToken)
    {
        return await context.ApplicationUsers
            .AnyAsync(u => u.Username == request.Username, cancellationToken);
    }
}