using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Shared.Auth;
using NcpAdminBlazor.Web.Application.Commands.Roles;

namespace NcpAdminBlazor.Web.Endpoints.Roles;

public sealed class CreateRoleEndpoint(IMediator mediator)
    : Endpoint<CreateRoleRequest, ResponseData<CreateRoleResponse>>
{
    public override void Configure()
    {
        Post("/api/roles");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(CreateRoleRequest req, CancellationToken ct)
    {
        var command = new CreateRoleCommand(
            req.Name,
            req.Description,
            req.Status,
            req.PermissionCodes);

        var roleId = await mediator.Send(command, ct);
        await Send.OkAsync(new CreateRoleResponse(roleId).AsResponseData(), ct);
    }
}

public sealed class CreateRoleRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Status { get; init; } = 1;
    public List<string> PermissionCodes { get; init; } = [];
}

public sealed record CreateRoleResponse(RoleId RoleId);

public sealed class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
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