using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.ApiService.Application.Queries.UsersManagement;

namespace NcpAdminBlazor.ApiService.Application.Commands.UsersManagement;

public record UpdateUserInfoCommand(
    UserId UserId,
    string Username,
    string RealName,
    string Email,
    string Phone,
    List<RoleId> RoleIds) : ICommand;

public class UpdateUserInfoCommandValidator : AbstractValidator<UpdateUserInfoCommand>
{
    public UpdateUserInfoCommandValidator(IMediator mediator)
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名不能超过50个字符");

        RuleFor(x => new { x.UserId, x.Username })
            .MustAsync(async (model, cancellationToken) =>
                !await mediator.Send(new CheckUserExistsByUsernameExceptIdQuery(model.Username, model.UserId),
                    cancellationToken))
            .WithMessage("用户名已存在");

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

        RuleFor(x => x.RoleIds)
            .NotNull().WithMessage("角色列表不能为空");
    }
}

public class UpdateUserInfoCommandHandler(IUserRepository userRepository)
    : ICommandHandler<UpdateUserInfoCommand>
{
    public async Task Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken)
                   ?? throw new KnownException($"未找到用户，UserId = {request.UserId}");

        user.UpdateInfo(request.Username, request.RealName, request.Email, request.Phone,
            request.RoleIds);
    }
}