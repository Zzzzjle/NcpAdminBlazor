using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Web.Application.Commands.Users;

namespace NcpAdminBlazor.Web.Endpoints.Users;

[Tags("User")]
[HttpPost("/api/user")]
public sealed class CreateUserEndpoint(IMediator mediator)
    : Endpoint<CreateUserRequest, ResponseData<CreateUserResponse>>
{
    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var command = new CreateUserCommand(
            req.Username?.Trim() ?? string.Empty,
            req.Password?.Trim() ?? string.Empty,
            req.RealName?.Trim() ?? string.Empty,
            req.Email?.Trim() ?? string.Empty,
            req.Phone?.Trim() ?? string.Empty,
            req.Status);

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
    public int Status { get; init; } = 1;
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

        RuleFor(x => x.Status)
            .Must(status => status is 0 or 1)
            .WithMessage("用户状态必须是0或1");
    }
}

internal sealed class CreateUserSummary : Summary<CreateUserEndpoint, CreateUserRequest>
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
                Status = 1
            },
            "创建用户示例"));
        ResponseExamples.Add(201, new CreateUserResponse(new ApplicationUserId(1)).AsResponseData());
    }
}