using FluentValidation;
using NcpAdminBlazor.Client.Models;

namespace NcpAdminBlazor.Client.Pages.Applications.User.Validators;

public class UserAddModelValidator : FormModelValidator<NcpAdminBlazorWebEndpointsUsersCreateUserRequest>
{
    public UserAddModelValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名长度不能超过50个字符");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("电子邮件不能为空")
            .EmailAddress().WithMessage("电子邮件格式不正确")
            .MaximumLength(100).WithMessage("电子邮件长度不能超过100个字符");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("电话号码长度不能超过20个字符");

        RuleFor(x => x.RealName)
            .MaximumLength(100).WithMessage("真实姓名长度不能超过100个字符");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码长度不能少于6个字符")
            .MaximumLength(100).WithMessage("密码长度不能超过100个字符");
    }
}