using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Web.Application.Commands.Menus;

namespace NcpAdminBlazor.Web.Endpoints.MenusManagement;

public sealed class CreateMenuEndpoint(IMediator mediator)
    : Endpoint<CreateMenuRequest, ResponseData<CreateMenuResponse>>
{
    public override void Configure()
    {
        Post("/api/menus");
        Description(d => d.WithTags("Menu"));
    }

    public override async Task HandleAsync(CreateMenuRequest req, CancellationToken ct)
    {
        var command = new CreateMenuCommand(
            req.ParentId,
            req.Title,
            req.Type,
            req.Order,
            req.IsDisabled,
            req.Icon,
            req.PageKey,
            req.Path,
            req.PermissionCode);

        var menuId = await mediator.Send(command, ct);
        await Send.OkAsync(new CreateMenuResponse(menuId).AsResponseData(), ct);
    }
}

public sealed class CreateMenuRequest
{
    public MenuId ParentId { get; init; } = MenuId.Root;
    public string Title { get; init; } = string.Empty;
    public MenuType Type { get; init; }
    public int Order { get; init; } = 0;
    public bool IsDisabled { get; init; } = false;
    public string Icon { get; init; } = string.Empty;
    public string PageKey { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string PermissionCode { get; init; } = string.Empty;
}

public sealed record CreateMenuResponse(MenuId MenuId);

public sealed class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest>
{
    public CreateMenuRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("菜单标题不能为空")
            .MaximumLength(128).WithMessage("菜单标题不能超过128个字符");

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