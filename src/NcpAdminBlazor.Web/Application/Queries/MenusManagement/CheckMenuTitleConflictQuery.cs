using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.MenusManagement;

public sealed record CheckMenuTitleConflictQuery(MenuId MenuId, MenuId? ParentId, string Title) : IQuery<bool>;

public sealed class CheckMenuTitleConflictQueryValidator : AbstractValidator<CheckMenuTitleConflictQuery>
{
    public CheckMenuTitleConflictQueryValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单标识不能为空");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("菜单标题不能为空")
            .MaximumLength(128).WithMessage("菜单标题不能超过128个字符");
    }
}

public sealed class CheckMenuTitleConflictQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckMenuTitleConflictQuery, bool>
{
    public Task<bool> Handle(CheckMenuTitleConflictQuery request, CancellationToken cancellationToken)
    {
        return context.Menus
            .AsNoTracking()
            .Where(menu => !menu.IsDeleted && menu.Id != request.MenuId)
            .AnyAsync(menu =>
                menu.ParentId == request.ParentId &&
                menu.Title == request.Title,
                cancellationToken);
    }
}
