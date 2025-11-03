using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;
using NcpAdminBlazor.Web.Application.Commands.UsersManagement;

namespace NcpAdminBlazor.Web.Endpoints.UsersManagement;

public sealed class UpdateUserEndpoint(IMediator mediator) : Endpoint<UpdateUserRequest, ResponseData>
{
    public override void Configure()
    {
        Post("/api/user/{userId}/update");
        Description(x => x.WithTags("User"));
    }

    public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
    {
        var command = new UpdateUserInfoCommand(
            req.UserId,
            req.Username,
            req.RealName,
            req.Email,
            req.Phone,
            req.AssignedRoleIds);

        await mediator.Send(command, ct);
        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class UpdateUserRequest
{
    [RouteParam] public required UserId UserId { get; set; }

    public string Username { get; init; } = string.Empty;

    public string RealName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;
    public int Status { get; init; }
    public List<RoleId> AssignedRoleIds { get; init; } = [];
}

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名不能超过50个字符");

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

internal sealed class UpdateUserSummary : Summary<UpdateUserEndpoint, UpdateUserRequest>
{
    public UpdateUserSummary()
    {
        Summary = "更新用户信息";
        Description = "更新指定用户的基础信息";
        RequestExamples.Add(new RequestExample(
            new UpdateUserRequest
            {
                UserId = new UserId(Guid.NewGuid()),
                Username = "admin",
                RealName = "管理员",
                Email = "admin@example.com",
                Phone = "13800000000",
                Status = 0
            },
            "更新用户示例"));
        ResponseExamples.Add(200, true.AsResponseData());
    }
}