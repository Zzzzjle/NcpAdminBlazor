using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.ApiService.Application.Commands.UsersManagement;

public record DeleteUserCommand(UserId UserId) : ICommand;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");
    }
}

public class DeleteUserCommandHandler(IUserRepository userRepository)
    : ICommandHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken)
                   ?? throw new KnownException($"未找到用户，UserId = {request.UserId}");

        user.Delete();
    }
}
