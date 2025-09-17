using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands;

public record LoginUserCommand(string LoginName, string Password) : ICommand<LoginUserResult>;

public record LoginUserResult(ApplicationUserId UserId, string Name, string Email, string RealName, List<string> Roles, List<string> Permissions);

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.LoginName)
            .NotEmpty().WithMessage("登录名不能为空");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空");
    }
}

public class LoginUserCommandHandler(
    IApplicationUserRepository userRepository,
    ILogger<LoginUserCommandHandler> logger) 
    : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // 支持用户名、邮箱、手机号登录
        ApplicationUser? user = null;
        
        // 先尝试用户名
        user = await userRepository.GetByNameAsync(request.LoginName, cancellationToken);
        
        // 如果没找到，尝试邮箱
        if (user == null && IsEmail(request.LoginName))
        {
            user = await userRepository.GetByEmailAsync(request.LoginName, cancellationToken);
        }
        
        // 如果没找到，尝试手机号
        if (user == null && IsPhone(request.LoginName))
        {
            user = await userRepository.GetByPhoneAsync(request.LoginName, cancellationToken);
        }
        
        if (user == null)
        {
            throw new KnownException("用户不存在");
        }
        
        // 验证密码
        var passwordHash = GeneratePasswordHash(request.Password, user.PasswordSalt);
        if (!user.VerifyLogin(passwordHash))
        {
            throw new KnownException("密码错误");
        }
        
        logger.LogInformation("User logged in successfully, UserId: {UserId}, Name: {Name}", user.Id, user.Name);
        
        // 返回登录结果
        var roles = user.Roles.Select(r => r.RoleName).ToList();
        var permissions = user.Permissions.Select(p => p.PermissionCode).ToList();
        
        return new LoginUserResult(user.Id, user.Name, user.Email, user.RealName, roles, permissions);
    }
    
    private static bool IsEmail(string input)
    {
        return input.Contains('@') && input.Contains('.');
    }
    
    private static bool IsPhone(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, @"^1[3-9]\d{9}$");
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