using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Web.Application.Queries.Users;

namespace NcpAdminBlazor.Web.Application.Commands.Users;

public record CreateUserCommand(
    string Username,
    string Password,
    string RealName,
    string Email,
    string Phone,
    List<UserRole> Roles,
    List<UserMenuPermission> MenuPermissions) : ICommand<ApplicationUserId>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IMediator mediator)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名不能超过50个字符")
            .MustAsync(async (username, cancellationToken) =>
                !await mediator.Send(new CheckUserExistsByUsernameQuery(username), cancellationToken))
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
            .MaximumLength(100).WithMessage("邮箱不能超过100个字符");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("手机号不能为空")
            .MaximumLength(20).WithMessage("手机号不能超过20个字符");

        RuleFor(x => x.Roles)
            .NotNull().WithMessage("角色列表不能为空");

        RuleFor(x => x.MenuPermissions)
            .NotNull().WithMessage("菜单权限列表不能为空");
    }
}

public class CreateUserCommandHandler(IApplicationUserRepository userRepository)
    : ICommandHandler<CreateUserCommand, ApplicationUserId>
{
    public async Task<ApplicationUserId> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var roles = new List<UserRole>(request.Roles);
        var menuPermissions = new List<UserMenuPermission>(request.MenuPermissions);

        var user = new ApplicationUser(
            request.Username,
            request.Password,
            request.RealName,
            request.Email,
            request.Phone,
            roles,
            menuPermissions);
        await userRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}
