using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.ApiService.Application.Commands.RolesManagement;

namespace NcpAdminBlazor.ApiService.Endpoints.RolesManagement;

public sealed class DeleteRoleEndpoint(IMediator mediator)
    : Endpoint<DeleteRoleRequest, ResponseData>
{
    public override void Configure()
    {
        Delete("/api/roles/{roleId}");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(DeleteRoleRequest req, CancellationToken ct)
    {
        var command = new DeleteRoleCommand(req.RoleId);
        await mediator.Send(command, ct);
        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class DeleteRoleRequest
{
    [RouteParam] public required RoleId RoleId { get; init; }
}

public sealed class DeleteRoleRequestValidator : AbstractValidator<DeleteRoleRequest>
{
    public DeleteRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}