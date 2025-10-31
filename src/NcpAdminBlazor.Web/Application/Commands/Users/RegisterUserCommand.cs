using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Web.Application.Queries.Users;

namespace NcpAdminBlazor.Web.Application.Commands.Users;

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
    IApplicationUserRepository userRepository)
    : ICommandHandler<RegisterUserCommand, ApplicationUserId>
{
    public async Task<ApplicationUserId> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser(
            username: request.Username,
            password: request.Password,
            string.Empty,
            string.Empty,
            string.Empty,
            [],
            []
        );

        await userRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}