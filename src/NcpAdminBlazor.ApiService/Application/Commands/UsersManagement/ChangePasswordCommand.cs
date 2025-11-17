using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.ApiService.Application.Commands.UsersManagement;

public record ChangePasswordCommand(UserId UserId, string OldPassword, string NewPassword) : ICommand;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");

        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("旧密码不能为空");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("新密码不能为空")
            .MinimumLength(6).WithMessage("新密码长度不能少于6位")
            .MaximumLength(50).WithMessage("新密码长度不能超过50位");
    }
}

public class ChangePasswordCommandHandler(
    IUserRepository userRepository)
    : ICommandHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken)
                   ?? throw new KnownException($"用户不存在，UserId = {request.UserId}");

        user.ChangePassword(request.OldPassword, request.NewPassword);
    }
}