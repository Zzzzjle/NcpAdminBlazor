using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Web.Application.Queries.MenusManagement;

namespace NcpAdminBlazor.Web.Application.Commands.Menus;

public sealed record CreateMenuCommand(
    MenuId ParentId,
    string Title,
    MenuType Type,
    int Order,
    bool IsDisabled,
    string Icon,
    string PageKey,
    string Path,
    string PermissionCode) : ICommand<MenuId>;

public sealed class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
{
    public CreateMenuCommandValidator(IMediator mediator)
    {
        RuleFor(x => x.ParentId)
            .NotEmpty().WithMessage("父级菜单ID不能为空")
            .MustAsync(async (parentId, cancellationToken) =>
            {
                if (parentId == MenuId.Root) return true;
                return !await mediator.Send(new CheckMenuExistsByIdQuery(parentId), cancellationToken);
            }).WithMessage("父级菜单不存在");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("菜单标题不能为空")
            .MaximumLength(128).WithMessage("菜单标题不能超过128个字符")
            .MustAsync(async (command, title, cancellationToken) =>
                !await mediator.Send(new CheckMenuTitleExistsQuery(command.ParentId, title), cancellationToken))
            .WithMessage("菜单标题已存在");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("菜单类型不正确");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("排序值必须大于等于0");

        RuleFor(x => x.Icon)
            .MaximumLength(64).WithMessage("图标长度不能超过64个字符")
            .When(x => !string.IsNullOrWhiteSpace(x.Icon));

        RuleFor(x => x.PageKey)
            .MaximumLength(128).WithMessage("页面Key不能超过128个字符")
            .When(x => !string.IsNullOrWhiteSpace(x.PageKey));

        RuleFor(x => x.Path)
            .MaximumLength(256).WithMessage("菜单路径不能超过256个字符")
            .When(x => !string.IsNullOrWhiteSpace(x.Path));

        RuleFor(x => x.PermissionCode)
            .MaximumLength(128).WithMessage("权限编码不能超过128个字符")
            .When(x => !string.IsNullOrWhiteSpace(x.PermissionCode));
    }
}

public sealed class CreateMenuCommandHandler(IMenuRepository menuRepository)
    : ICommandHandler<CreateMenuCommand, MenuId>
{
    public async Task<MenuId> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = new Menu(
            request.ParentId,
            request.Title,
            request.Type,
            request.Order,
            request.IsDisabled,
            request.Icon,
            request.PageKey,
            request.Path,
            request.PermissionCode);

        await menuRepository.AddAsync(menu, cancellationToken);
        return menu.Id;
    }
}