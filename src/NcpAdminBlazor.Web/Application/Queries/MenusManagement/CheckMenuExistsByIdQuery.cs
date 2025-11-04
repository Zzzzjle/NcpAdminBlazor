using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.MenusManagement;

public sealed record CheckMenuExistsByIdQuery(MenuId Id) : IQuery<bool>;

public sealed class CheckMenuExistsByIdQueryValidator : AbstractValidator<CheckMenuExistsByIdQuery>
{
    public CheckMenuExistsByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("菜单ID不能为空");
    }
}

public sealed class CheckMenuExistsByIdQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckMenuExistsByIdQuery, bool>
{
    public Task<bool> Handle(CheckMenuExistsByIdQuery request, CancellationToken cancellationToken)
    {
        return context.Menus
            .AsNoTracking()
            .Where(menu => !menu.IsDeleted)
            .AnyAsync(menu => menu.Id == request.Id,
                cancellationToken);
    }
}