using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Web.Application.Queries.UsersManagement;

namespace NcpAdminBlazor.Web.Application.Commands.UsersManagement;

public record RegisterUserCommand(
    string Username,
    string Password
) : ICommand<UserId>;

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
    IUserRepository userRepository)
    : ICommandHandler<RegisterUserCommand, UserId>
{
    public async Task<UserId> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User(
            username: request.Username,
            password: request.Password,
            string.Empty,
            string.Empty,
            string.Empty,
            []
        );

        await userRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}