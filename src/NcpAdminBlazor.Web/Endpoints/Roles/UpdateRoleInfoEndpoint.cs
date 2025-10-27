using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Shared.Auth;
using NcpAdminBlazor.Web.Application.Commands.Roles;

namespace NcpAdminBlazor.Web.Endpoints.Roles;

public sealed class UpdateRoleInfoEndpoint(IMediator mediator)
    : Endpoint<UpdateRoleInfoRequest, ResponseData>
{
    public override void Configure()
    {
        Post("/api/roles/{roleId:long}/info");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(UpdateRoleInfoRequest req, CancellationToken ct)
    {
        var command = new UpdateRoleInfoCommand(req.RoleId, req.Name, req.Description, req.Status);
        await mediator.Send(command, ct);
        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class UpdateRoleInfoRequest
{
    [RouteParam] public required RoleId RoleId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Status { get; init; } = 1;
}

public sealed class UpdateRoleInfoRequestValidator : AbstractValidator<UpdateRoleInfoRequest>
{
    public UpdateRoleInfoRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("角色名称不能为空")
            .MaximumLength(50).WithMessage("角色名称不能超过50个字符");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("角色描述不能为空")
            .MaximumLength(200).WithMessage("角色描述不能超过200个字符");

        RuleFor(x => x.Status)
            .Must(status => status is 0 or 1)
            .WithMessage("角色状态必须是0或1");
    }
}
