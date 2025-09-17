using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands;

public record ChangePasswordCommand(ApplicationUserId UserId, string OldPassword, string NewPassword) : ICommand;

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
    IApplicationUserRepository userRepository,
    ILogger<ChangePasswordCommandHandler> logger) 
    : ICommandHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken) 
                   ?? throw new KnownException($"用户不存在，UserId = {request.UserId}");
        
        // 生成旧密码哈希进行验证
        var oldPasswordHash = GeneratePasswordHash(request.OldPassword, user.PasswordSalt);
        
        // 生成新密码盐值和哈希
        var newSalt = GeneratePasswordSalt();
        var newPasswordHash = GeneratePasswordHash(request.NewPassword, newSalt);
        
        // 调用聚合根方法修改密码
        user.ChangePassword(oldPasswordHash, newPasswordHash, newSalt);
        
        await userRepository.UpdateAsync(user, cancellationToken);
        logger.LogInformation("User password changed successfully, UserId: {UserId}", user.Id);
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