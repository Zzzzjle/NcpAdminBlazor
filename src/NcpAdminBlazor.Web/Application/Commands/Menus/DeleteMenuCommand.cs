using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands.Menus;

public sealed record DeleteMenuCommand(MenuId MenuId) : ICommand;

public sealed class DeleteMenuCommandValidator : AbstractValidator<DeleteMenuCommand>
{
    public DeleteMenuCommandValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单标识不能为空");
    }
}

public sealed class DeleteMenuCommandHandler(IMenuRepository menuRepository)
    : ICommandHandler<DeleteMenuCommand>
{
    public async Task Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetAsync(request.MenuId, cancellationToken)
                   ?? throw new KnownException($"未找到菜单，MenuId = {request.MenuId}");

        menu.Delete();
    }
}
