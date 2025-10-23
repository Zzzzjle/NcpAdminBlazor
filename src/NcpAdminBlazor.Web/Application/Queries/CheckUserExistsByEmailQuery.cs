using Microsoft.EntityFrameworkCore;

namespace NcpAdminBlazor.Web.Application.Queries;

public record CheckUserExistsByEmailQuery(string Email) : IQuery<bool>;

public class CheckUserExistsByEmailQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckUserExistsByEmailQuery, bool>
{
    public async Task<bool> Handle(CheckUserExistsByEmailQuery request, CancellationToken cancellationToken)
    {
        return await context.ApplicationUsers
            .AnyAsync(user => user.Email == request.Email && !user.IsDeleted, cancellationToken);
    }
}
