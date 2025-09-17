using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands;

public record RegisterUserCommand(
    string Name, 
    string Email, 
    string Password, 
    string Phone, 
    string RealName
) : ICommand<ApplicationUserId>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名不能超过50个字符");
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确")
            .MaximumLength(100).WithMessage("邮箱不能超过100个字符");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码长度不能少于6位")
            .MaximumLength(50).WithMessage("密码长度不能超过50位");
            
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("手机号不能为空")
            .Matches(@"^1[3-9]\d{9}$").WithMessage("手机号格式不正确");
            
        RuleFor(x => x.RealName)
            .NotEmpty().WithMessage("真实姓名不能为空")
            .MaximumLength(50).WithMessage("真实姓名不能超过50个字符");
    }
}

public class RegisterUserCommandHandler(
    IApplicationUserRepository userRepository,
    ILogger<RegisterUserCommandHandler> logger) 
    : ICommandHandler<RegisterUserCommand, ApplicationUserId>
{
    public async Task<ApplicationUserId> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 检查用户名是否已存在
        var existingUserByName = await userRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingUserByName != null)
        {
            throw new KnownException("用户名已存在");
        }
        
        // 检查邮箱是否已存在
        var existingUserByEmail = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUserByEmail != null)
        {
            throw new KnownException("邮箱已存在");
        }
        
        // 检查手机号是否已存在
        var existingUserByPhone = await userRepository.GetByPhoneAsync(request.Phone, cancellationToken);
        if (existingUserByPhone != null)
        {
            throw new KnownException("手机号已存在");
        }
        
        // 生成密码盐值和哈希
        var salt = GeneratePasswordSalt();
        var passwordHash = GeneratePasswordHash(request.Password, salt);
        
        // 创建用户
        var user = new ApplicationUser(
            name: request.Name,
            phone: request.Phone,
            passwordHash: passwordHash,
            passwordSalt: salt,
            roles: new List<ApplicationUserRole>(),
            permissions: new List<ApplicationUserPermission>(),
            realName: request.RealName,
            status: 1, // 默认启用状态
            email: request.Email
        );
        
        await userRepository.AddAsync(user, cancellationToken);
        logger.LogInformation("User registered successfully, UserId: {UserId}, Name: {Name}", user.Id, user.Name);
        
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