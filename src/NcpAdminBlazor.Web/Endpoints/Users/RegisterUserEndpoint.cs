using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Web.Application.Commands;

namespace NcpAdminBlazor.Web.Endpoints.Users;

public sealed class RegisterUserEndpoint(IMediator mediator)
    : Endpoint<RegisterUserRequest, ResponseData<RegisterUserResponse>>
{
    public override void Configure()
    {
        Post("/api/user/create");
        Description(x => x.WithTags("User"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterUserRequest r, CancellationToken ct)
    {
        var cmd = new RegisterUserCommand(r.Username, r.Password);
        var result = await mediator.Send(cmd, ct);
        var res = new RegisterUserResponse(result);
        await Send.OkAsync(res.AsResponseData(), ct);
    }
}

/// <summary>
/// 创建用户请求Payload
/// </summary>
/// <param name="Username">用户名</param>
public sealed record RegisterUserRequest(string Username, string Password);

/// <summary>
/// 创建用户的响应数据
/// </summary>
/// <param name="UserId">用户ID</param>
public sealed record RegisterUserResponse(ApplicationUserId UserId);

// webapi请求对象验证器
internal sealed class CreateUserValidator : Validator<RegisterUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
    }
}

// webapi描述内容
internal sealed class RegisterUserSummary : Summary<RegisterUserEndpoint, RegisterUserRequest>
{
    public RegisterUserSummary()
    {
        Summary = "创建用户";
        // 给webapi文档添加请求响应示例
        RequestExamples.Add(new RequestExample(new RegisterUserRequest("admin", "123"), "创建用户示例1"));
        ResponseExamples.Add(200, new RegisterUserResponse(new ApplicationUserId(123)).AsResponseData());
    }
}