using Microsoft.EntityFrameworkCore;

namespace NcpAdminBlazor.Web.Application.Queries;

public record CheckUserExistsByPhoneQuery(string Phone) : IQuery<bool>;

public class CheckUserExistsByPhoneQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckUserExistsByPhoneQuery, bool>
{
    public async Task<bool> Handle(CheckUserExistsByPhoneQuery request, CancellationToken cancellationToken)
    {
        return await context.ApplicationUsers
            .AnyAsync(user => user.Phone == request.Phone && !user.IsDeleted, cancellationToken);
    }
}
