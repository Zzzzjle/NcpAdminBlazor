using Microsoft.EntityFrameworkCore;

namespace NcpAdminBlazor.Web.Application.Queries.Roles;

public record CheckRoleExistsByNameQuery(string Name) : IQuery<bool>;

public class CheckRoleExistsByNameQueryValidator : AbstractValidator<CheckRoleExistsByNameQuery>
{
    public CheckRoleExistsByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("角色名称不能为空");
    }
}

public class CheckRoleExistsByNameQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckRoleExistsByNameQuery, bool>
{
    public async Task<bool> Handle(CheckRoleExistsByNameQuery request, CancellationToken cancellationToken)
    {
        return await context.Roles
            .AnyAsync(role => role.Name == request.Name, cancellationToken);
    }
}