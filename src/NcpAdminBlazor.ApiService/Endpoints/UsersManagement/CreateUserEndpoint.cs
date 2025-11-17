using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;
using NcpAdminBlazor.ApiService.Application.Commands.UsersManagement;

namespace NcpAdminBlazor.ApiService.Endpoints.UsersManagement;

public sealed class CreateUserEndpoint(IMediator mediator)
    : Endpoint<CreateUserRequest, ResponseData<CreateUserResponse>>
{
    public override void Configure()
    {
        Post("/api/users");
        Description(x => x.WithTags("User"));
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var command = new CreateUserCommand(
            req.Username,
            req.Password,
            req.RealName,
            req.Email,
            req.Phone,
            req.AssignedRoleIds);

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
    public List<RoleId> AssignedRoleIds { get; init; } = [];
}

public sealed record CreateUserResponse(UserId UserId);

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
    }
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
            },
            "创建用户示例"));
        ResponseExamples.Add(201, new CreateUserResponse(new UserId(Guid.NewGuid())).AsResponseData());
    }
}