using System;
using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Web.Application.Commands.Users;

namespace NcpAdminBlazor.Web.Endpoints.Users;

[Tags("User")]
[HttpPost("/api/user")]
public sealed class CreateUserEndpoint(IMediator mediator)
    : Endpoint<CreateUserRequest, ResponseData<CreateUserResponse>>
{
    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var roleItems = req.Roles ?? [];
        var roles = roleItems.Select(role =>
        {
            var userRole = new UserRole(role.RoleId, role.RoleName);
            userRole.UpdateUserRoleInfo(role.RoleName, role.IsDisabled);
            return userRole;
        }).ToList();

        var permissionItems = req.MenuPermissions ?? [];
        var menuPermissions = permissionItems
            .Select(permission => new UserMenuPermission(permission.MenuId, permission.SourceRoleId, permission.PermissionCode))
            .ToList();

        var command = new CreateUserCommand(
            req.Username,
            req.Password,
            req.RealName,
            req.Email,
            req.Phone,
            roles,
            menuPermissions);

        var userId = await mediator.Send(command, ct);
        await Send.OkAsync(new CreateUserResponse(userId).AsResponseData(), ct);
    }
}

public sealed class CreateUserRequest
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string RealName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public List<CreateUserRoleItem> Roles { get; init; } = [];
    public List<CreateUserMenuPermissionItem> MenuPermissions { get; init; } = [];
}

public sealed record CreateUserResponse(ApplicationUserId UserId);

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名长度不能超过50个字符");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MaximumLength(50).WithMessage("密码长度不能超过50位");

        RuleFor(x => x.RealName)
            .NotEmpty().WithMessage("姓名不能为空")
            .MaximumLength(50).WithMessage("姓名不能超过50个字符");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确")
            .MaximumLength(100).WithMessage("邮箱不能超过100个字符");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("手机号不能为空")
            .MaximumLength(20).WithMessage("手机号不能超过20个字符");

        RuleFor(x => x.Roles)
            .NotNull().WithMessage("角色列表不能为空");

        RuleForEach(x => x.Roles).ChildRules(role =>
        {
            role.RuleFor(r => r.RoleId)
                .NotEmpty().WithMessage("角色ID不能为空");

            role.RuleFor(r => r.RoleName)
                .NotEmpty().WithMessage("角色名称不能为空")
                .MaximumLength(50).WithMessage("角色名称不能超过50个字符");
        });

        RuleForEach(x => x.MenuPermissions).ChildRules(permission =>
        {
            permission.RuleFor(p => p.MenuId)
                .NotEmpty().WithMessage("菜单ID不能为空");

            permission.RuleFor(p => p.SourceRoleId)
                .NotEmpty().WithMessage("菜单权限来源角色不能为空");

            permission.RuleFor(p => p.PermissionCode)
                .NotEmpty().WithMessage("权限编码不能为空")
                .MaximumLength(100).WithMessage("权限编码不能超过100个字符");
        });
    }
}

public sealed class CreateUserRoleItem
{
    public required RoleId RoleId { get; init; }

    public string RoleName { get; init; } = string.Empty;

    public bool IsDisabled { get; init; }
}

public sealed class CreateUserMenuPermissionItem
{
    public required MenuId MenuId { get; init; }

    public required RoleId SourceRoleId { get; init; }

    public string PermissionCode { get; init; } = string.Empty;
}

public sealed class CreateUserSummary : Summary<CreateUserEndpoint, CreateUserRequest>
{
    public CreateUserSummary()
    {
        Summary = "创建用户";
        Description = "创建一个新的后台用户";
        RequestExamples.Add(new RequestExample(
            new CreateUserRequest
            {
                Username = "admin",
                Password = "123456",
                RealName = "管理员",
                Email = "admin@example.com",
                Phone = "13800000000",
                Roles =
                [
                    new CreateUserRoleItem
                    {
                        RoleId = new RoleId(Guid.NewGuid()),
                        RoleName = "系统管理员",
                        IsDisabled = false
                    }
                ],
                MenuPermissions = []
            },
            "创建用户示例"));
        ResponseExamples.Add(201, new CreateUserResponse(new ApplicationUserId(Guid.NewGuid())).AsResponseData());
    }
}