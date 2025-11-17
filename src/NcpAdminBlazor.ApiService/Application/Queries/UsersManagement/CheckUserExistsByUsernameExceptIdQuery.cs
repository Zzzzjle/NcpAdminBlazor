using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.UsersManagement;

public record CheckUserExistsByUsernameExceptIdQuery(string Username, UserId UserId) : IQuery<bool>;

public class CheckUserExistsByUsernameExceptIdQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckUserExistsByUsernameExceptIdQuery, bool>
{
    public async Task<bool> Handle(CheckUserExistsByUsernameExceptIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Users
            .AnyAsync(u => u.Username == request.Username && u.Id != request.UserId, cancellationToken);
    }
}