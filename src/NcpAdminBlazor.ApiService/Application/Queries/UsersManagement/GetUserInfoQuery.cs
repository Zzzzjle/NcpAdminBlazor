using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.UsersManagement;

public record GetUserInfoQuery(UserId UserId) : IQuery<UserInfoDto>;

public record UserInfoDto(
    UserId Id,
    string Username,
    string Email,
    string Phone,
    string RealName,
    DateTimeOffset CreatedAt,
    List<UserRoleSummaryDto> Roles
);

public record UserRoleSummaryDto(RoleId RoleId, string RoleName, bool IsDisabled);

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
        var user = await context.Users
            .Where(u => u.Id == request.UserId && !u.IsDeleted)
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.Phone,
                u.RealName,
                u.CreatedAt,
                u.AssignedRoleIds
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KnownException($"用户不存在，UserId = {request.UserId}");

        var assignedRoleIds = user.AssignedRoleIds.ToList();

        var roleSummaries = await context.Roles
            .Where(role => assignedRoleIds.Contains(role.Id))
            .Select(role => new UserRoleSummaryDto(role.Id, role.Name, role.IsDisabled))
            .ToListAsync(cancellationToken);

        return new UserInfoDto(
            user.Id,
            user.Username,
            user.Email,
            user.Phone,
            user.RealName,
            user.CreatedAt,
            roleSummaries);
    }
}