using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands.Users;

public record UpdateUserInfoCommand(
    ApplicationUserId UserId,
    string Username,
    string RealName,
    string Email,
    string Phone,
    int Status) : ICommand;

public class UpdateUserInfoCommandValidator : AbstractValidator<UpdateUserInfoCommand>
{
    public UpdateUserInfoCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");

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

        RuleFor(x => x.Status)
            .Must(status => status is 0 or 1)
            .WithMessage("用户状态必须是0或1");
    }
}

public class UpdateUserInfoCommandHandler(IApplicationUserRepository userRepository)
    : ICommandHandler<UpdateUserInfoCommand>
{
    public async Task Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken)
                   ?? throw new KnownException($"未找到用户，UserId = {request.UserId}");

        var normalizedUsername = request.Username.Trim();
        var normalizedRealName = request.RealName.Trim();
        var normalizedEmail = request.Email.Trim();
        var normalizedPhone = request.Phone.Trim();

        if (!string.Equals(user.Username, normalizedUsername, StringComparison.OrdinalIgnoreCase))
        {
            var existingByName = await userRepository.GetByNameAsync(normalizedUsername, cancellationToken);
            if (existingByName is not null && existingByName.Id != user.Id)
            {
                throw new KnownException("用户名已存在");
            }
        }

        if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
        {
            var existingByEmail = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (existingByEmail is not null && existingByEmail.Id != user.Id)
            {
                throw new KnownException("邮箱已存在");
            }
        }

        if (!string.Equals(user.Phone, normalizedPhone, StringComparison.Ordinal))
        {
            var existingByPhone = await userRepository.GetByPhoneAsync(normalizedPhone, cancellationToken);
            if (existingByPhone is not null && existingByPhone.Id != user.Id)
            {
                throw new KnownException("手机号已存在");
            }
        }

        user.UpdateProfile(normalizedUsername, normalizedRealName, normalizedEmail, normalizedPhone, request.Status);
    }
}
