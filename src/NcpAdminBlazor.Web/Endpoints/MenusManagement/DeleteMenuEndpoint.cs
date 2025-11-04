using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Web.Application.Commands.Menus;

namespace NcpAdminBlazor.Web.Endpoints.MenusManagement;

public sealed class DeleteMenuEndpoint(IMediator mediator)
    : Endpoint<DeleteMenuRequest, ResponseData>
{
    public override void Configure()
    {
        Delete("/api/menus/{menuId}");
        Description(d => d.WithTags("Menu"));
    }

    public override async Task HandleAsync(DeleteMenuRequest req, CancellationToken ct)
    {
        var command = new DeleteMenuCommand(req.MenuId);
        await mediator.Send(command, ct);
        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class DeleteMenuRequest
{
    [RouteParam] public required MenuId MenuId { get; init; }
}

public sealed class DeleteMenuRequestValidator : AbstractValidator<DeleteMenuRequest>
{
    public DeleteMenuRequestValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单标识不能为空");
    }
}
