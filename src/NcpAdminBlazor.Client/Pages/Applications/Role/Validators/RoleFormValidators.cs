using FluentValidation;
using NcpAdminBlazor.Client.Pages.Applications.Role.Models;

namespace NcpAdminBlazor.Client.Pages.Applications.Role.Validators;

public class RoleAddModelValidator : FormModelValidator<RoleFormModel>
{
    public RoleAddModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("角色名称不能为空")
            .MaximumLength(50).WithMessage("角色名称长度不能超过50个字符");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("角色描述不能为空")
            .MaximumLength(200).WithMessage("角色描述长度不能超过200个字符");
    }
}

public class RoleEditModelValidator : FormModelValidator<RoleFormModel>
{
    public RoleEditModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("角色名称不能为空")
            .MaximumLength(50).WithMessage("角色名称长度不能超过50个字符");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("角色描述不能为空")
            .MaximumLength(200).WithMessage("角色描述长度不能超过200个字符");
    }
}
