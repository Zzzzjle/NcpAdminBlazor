using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Shared.Auth;
using NcpAdminBlazor.Web.Application.Commands.Roles;

namespace NcpAdminBlazor.Web.Endpoints.Roles;

public sealed class UpdateRolePermissionsEndpoint(IMediator mediator)
    : Endpoint<UpdateRolePermissionsRequest, ResponseData>
{
    public override void Configure()
    {
        Post("/api/roles/{roleId:long}/permissions");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(UpdateRolePermissionsRequest req, CancellationToken ct)
    {
        var command = new UpdateRolePermissionsCommand(req.RoleId, req.PermissionCodes);
        await mediator.Send(command, ct);
        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class UpdateRolePermissionsRequest
{
    [RouteParam] public required RoleId RoleId { get; init; }
    public List<string> PermissionCodes { get; init; } = [];
}

public sealed class UpdateRolePermissionsRequestValidator : AbstractValidator<UpdateRolePermissionsRequest>
{
    public UpdateRolePermissionsRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleForEach(x => x.PermissionCodes)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("权限代码不能为空")
            .Must(code => !string.IsNullOrWhiteSpace(code))
            .WithMessage("权限代码不能为空");
    }
}