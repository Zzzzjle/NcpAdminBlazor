using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands;

public record LoginUserCommand(string Username, string Password) : ICommand<LoginUserResult>;

public record LoginUserResult(ApplicationUserId UserId, string UserName);

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("登录名不能为空");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空");
    }
}

public class LoginUserCommandHandler(
    IApplicationUserRepository userRepository)
    : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByNameAsync(request.Username, cancellationToken) ??
                   throw new KnownException("用户不存在");

        var passwordHash = GeneratePasswordHash(request.Password, user.PasswordSalt);

        return !user.VerifyLogin(passwordHash)
            ? throw new KnownException("密码错误")
            : new LoginUserResult(user.Id, user.Username);
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