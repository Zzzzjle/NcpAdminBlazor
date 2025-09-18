using Microsoft.EntityFrameworkCore;

namespace NcpAdminBlazor.Web.Application.Queries;

public record CheckUserExistsByUsernameQuery(string Username) : IQuery<bool>;

public class CheckUserExistsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckUserExistsByUsernameQuery, bool>
{
    public async Task<bool> Handle(CheckUserExistsByUsernameQuery request, CancellationToken cancellationToken)
    {
        return await context.ApplicationUsers
            .AnyAsync(u => u.Username == request.Username && !u.IsDeleted, cancellationToken);
    }
}