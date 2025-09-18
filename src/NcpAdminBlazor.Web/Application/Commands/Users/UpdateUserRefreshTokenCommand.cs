using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands;

/// <summary>
/// 更新用户的刷新令牌
/// </summary>
/// <param name="UserId">用户ID</param>
/// <param name="RefreshToken">刷新令牌</param>
/// <param name="RefreshExpiry">刷新令牌到期时间</param>
public record UpdateUserRefreshTokenCommand(ApplicationUserId UserId, string RefreshToken, DateTimeOffset RefreshExpiry) : ICommand;

public class UpdateUserRefreshTokenCommandValidator : AbstractValidator<UpdateUserRefreshTokenCommand>
{
    public UpdateUserRefreshTokenCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("刷新令牌不能为空");

        RuleFor(x => x.RefreshExpiry)
            .Must(x => x > DateTimeOffset.UtcNow.AddMinutes(-1))
            .WithMessage("刷新令牌到期时间必须是将来时间");
    }
}

public class UpdateUserRefreshTokenCommandHandler(IApplicationUserRepository userRepository)
    : ICommandHandler<UpdateUserRefreshTokenCommand>
{
    public async Task Handle(UpdateUserRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken)
                   ?? throw new KnownException($"未找到用户，UserId = {request.UserId}");

        user.SetRefreshToken(request.RefreshToken, request.RefreshExpiry);

        // 按规范：命令处理器不调用 SaveChanges，框架自动处理
    }
}
