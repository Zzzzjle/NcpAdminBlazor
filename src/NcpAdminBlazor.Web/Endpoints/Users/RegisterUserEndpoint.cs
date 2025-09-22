using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Web.Application.Commands;

namespace NcpAdminBlazor.Web.Endpoints.Users;

// 定义webapi接口
public sealed class RegisterUserEndpoint(IMediator mediator)
    : Endpoint<RegisterUserRequest, ResponseData<RegisterUserResponse>>
{
    // api配置
    public override void Configure()
    {
        Post("/api/user/create"); // api路由
        Description(x => x.WithTags("User")); // 路由分组
        AllowAnonymous(); // 匿名访问，不调用则需要身份认证后才能访问
    }

    // 业务逻辑代码
    public override async Task HandleAsync(RegisterUserRequest r, CancellationToken c)
    {
        // var password = PasswordHelper.NewPassword();
        var password = "1231231";
        var cmd = new RegisterUserCommand(r.UserName, password);
        var result = await mediator.Send(cmd, c);
        var res = new RegisterUserResponse(result);
        await Send.OkAsync(res.AsResponseData(), c);

        // var payload = new UserInfoRequest(result.UserId);
        // await SendCreatedAtAsync<UserInfoEndpoint>(payload, cancellation: c);
    }
}

/// <summary>
/// 创建用户请求Payload
/// </summary>
/// <param name="UserName">用户名</param>
public sealed record RegisterUserRequest(string UserName);

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
        RuleFor(x => x.UserName).NotEmpty();
    }
}

// webapi描述内容
internal sealed class CreateUserSummary : Summary<RegisterUserEndpoint, RegisterUserRequest>
{
    public CreateUserSummary()
    {
        Summary = "创建用户";
        Description = "用户密码由系统随机生成";
        // 给webapi文档添加请求响应示例
        RequestExamples.Add(new RequestExample(new RegisterUserRequest("admin"), "创建用户示例1"));
        RequestExamples.Add(new RequestExample(new RegisterUserRequest("admin22"), "创建用户示例2"));
        ResponseExamples.Add(200, new RegisterUserResponse(new ApplicationUserId(123)).AsResponseData());
    }
}