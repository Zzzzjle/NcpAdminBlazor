using System.Security.Cryptography;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Web.Application.Queries;

namespace NcpAdminBlazor.Web.Application.Commands.Users;

public record CreateUserCommand(
    string Username,
    string Password,
    string RealName,
    string Email,
    string Phone,
    int Status) : ICommand<ApplicationUserId>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IMediator mediator)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名不能超过50个字符")
            .MustAsync(async (username, cancellationToken) =>
                !await mediator.Send(new CheckUserExistsByUsernameQuery(username.Trim()), cancellationToken))
            .WithMessage("用户名已存在");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MaximumLength(50).WithMessage("密码长度不能超过50位");

        RuleFor(x => x.RealName)
            .NotEmpty().WithMessage("姓名不能为空")
            .MaximumLength(50).WithMessage("姓名不能超过50个字符");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确")
            .MaximumLength(100).WithMessage("邮箱不能超过100个字符")
            .MustAsync(async (email, cancellationToken) =>
                !await mediator.Send(new CheckUserExistsByEmailQuery(email.Trim()), cancellationToken))
            .WithMessage("邮箱已存在");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("手机号不能为空")
            .MaximumLength(20).WithMessage("手机号不能超过20个字符")
            .MustAsync(async (phone, cancellationToken) =>
                !await mediator.Send(new CheckUserExistsByPhoneQuery(phone.Trim()), cancellationToken))
            .WithMessage("手机号已存在");

        RuleFor(x => x.Status)
            .Must(status => status is 0 or 1)
            .WithMessage("用户状态必须是0或1");
    }
}

public class CreateUserCommandHandler(IApplicationUserRepository userRepository)
    : ICommandHandler<CreateUserCommand, ApplicationUserId>
{
    public async Task<ApplicationUserId> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedUsername = request.Username.Trim();
        var normalizedRealName = request.RealName.Trim();
        var normalizedEmail = request.Email.Trim();
        var normalizedPhone = request.Phone.Trim();

        var salt = GeneratePasswordSalt();
        var passwordHash = GeneratePasswordHash(request.Password, salt);

        var user = new ApplicationUser(normalizedUsername, passwordHash, salt);
        user.UpdateProfile(normalizedUsername, normalizedRealName, normalizedEmail, normalizedPhone, request.Status);

        await userRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }

    private static string GeneratePasswordSalt()
    {
        var saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    private static string GeneratePasswordHash(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hashBytes = new byte[saltBytes.Length + passwordBytes.Length];

        Array.Copy(saltBytes, 0, hashBytes, 0, saltBytes.Length);
        Array.Copy(passwordBytes, 0, hashBytes, saltBytes.Length, passwordBytes.Length);

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(hashBytes);
        return Convert.ToBase64String(hash);
    }
}
