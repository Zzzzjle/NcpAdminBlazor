using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Infrastructure;
using NetCorePal.Extensions.AspNetCore;

namespace NcpAdminBlazor.Web.Application.Queries;

public record GetUserInfoQuery(ApplicationUserId UserId) : IQuery<UserInfoDto>;

public record UserInfoDto(
    ApplicationUserId Id,
    string Name,
    string Email,
    string Phone,
    string RealName,
    int Status,
    DateTimeOffset CreatedAt,
    List<UserRoleDto> Roles,
    List<string> Permissions
);

public record UserRoleDto(string RoleName, string RoleId);

public class GetUserInfoQueryValidator : AbstractValidator<GetUserInfoQuery>
{
    public GetUserInfoQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");
    }
}

public class GetUserInfoQueryHandler(ApplicationDbContext context) 
    : IQueryHandler<GetUserInfoQuery, UserInfoDto>
{
    public async Task<UserInfoDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await context.ApplicationUsers
            .Include(u => u.Roles)
            .Include(u => u.Permissions)
            .Where(u => u.Id == request.UserId && !u.IsDeleted)
            .Select(u => new UserInfoDto(
                u.Id,
                u.Name,
                u.Email,
                u.Phone,
                u.RealName,
                u.Status,
                u.CreatedAt,
                u.Roles.Select(r => new UserRoleDto(r.RoleName, r.RoleId.ToString())).ToList(),
                u.Permissions.Select(p => p.PermissionCode).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return user ?? throw new KnownException($"用户不存在，UserId = {request.UserId}");
    }
}