using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Web.Application.Queries;

namespace NcpAdminBlazor.Web.Application.Commands;

public record RegisterUserCommand(
    string Username,
    string Password
) : ICommand<ApplicationUserId>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(IMediator mediator)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50)
            .WithMessage("用户名不能超过50个字符")
            .MustAsync(async (name, cancellation) =>
                !await mediator.Send(new CheckUserExistsByUsernameQuery(name), cancellation))
            .WithMessage("用户名已存在");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MaximumLength(50).WithMessage("密码长度不能超过50位");
    }
}

public class RegisterUserCommandHandler(
    IApplicationUserRepository userRepository,
    ILogger<RegisterUserCommandHandler> logger)
    : ICommandHandler<RegisterUserCommand, ApplicationUserId>
{
    public async Task<ApplicationUserId> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 生成密码盐值和哈希
        var salt = GeneratePasswordSalt();
        var passwordHash = GeneratePasswordHash(request.Password, salt);

        // 创建用户
        var user = new ApplicationUser(
            username: request.Username,
            passwordHash: passwordHash,
            passwordSalt: salt
        );

        await userRepository.AddAsync(user, cancellationToken);
        logger.LogInformation("User registered successfully, UserId: {UserId}, Username: {Username}", user.Id,
            user.Username);

        return user.Id;
    }

    private static string GeneratePasswordSalt()
    {
        var saltBytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
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

        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(hashBytes);
        return Convert.ToBase64String(hash);
    }
}