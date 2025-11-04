using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.MenusManagement;

public sealed record CheckMenuTitleExistsQuery(MenuId? ParentId, string Title) : IQuery<bool>;

public sealed class CheckMenuTitleExistsQueryValidator : AbstractValidator<CheckMenuTitleExistsQuery>
{
    public CheckMenuTitleExistsQueryValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("菜单标题不能为空")
            .MaximumLength(128).WithMessage("菜单标题不能超过128个字符");
    }
}

public sealed class CheckMenuTitleExistsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckMenuTitleExistsQuery, bool>
{
    public Task<bool> Handle(CheckMenuTitleExistsQuery request, CancellationToken cancellationToken)
    {
        return context.Menus
            .AsNoTracking()
            .Where(menu => !menu.IsDeleted)
            .AnyAsync(menu =>
                menu.ParentId == request.ParentId &&
                menu.Title == request.Title,
                cancellationToken);
    }
}
