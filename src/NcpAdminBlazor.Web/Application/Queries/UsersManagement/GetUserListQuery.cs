using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.UsersManagement;

public record GetUserListQuery(
    string? Username,
    string? Email,
    string? Phone,
    string? RealName,
    IPageRequest PageRequest
) : IQuery<PagedData<UserListItemDto>>;

public record UserListItemDto(
    UserId Id,
    string Username,
    string Email,
    string Phone,
    string RealName,
    DateTimeOffset CreatedAt,
    List<RoleId> RoleIds
);

public class GetUserListQueryValidator : AbstractValidator<GetUserListQuery>
{
    public GetUserListQueryValidator()
    {
        RuleFor(x => x.PageRequest.PageIndex)
            .GreaterThan(0).WithMessage("页码必须大于0");

        RuleFor(x => x.PageRequest.PageSize)
            .InclusiveBetween(1, 100).WithMessage("每页条数必须在1-100之间");
    }
}

public class GetUserListQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetUserListQuery, PagedData<UserListItemDto>>
{
    public async Task<PagedData<UserListItemDto>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        var usersQuery = context.Users
            .Include(u => u.AssignedRoleIds)
            .Where(u => !u.IsDeleted)
            .WhereIf(!string.IsNullOrEmpty(request.Username), u => u.Username.Contains(request.Username!))
            .WhereIf(!string.IsNullOrEmpty(request.Email), u => u.Email.Contains(request.Email!))
            .WhereIf(!string.IsNullOrEmpty(request.Phone), u => u.Phone.Contains(request.Phone!))
            .WhereIf(!string.IsNullOrEmpty(request.RealName), u => u.RealName.Contains(request.RealName!))
            .Select(u => new UserListItemDto(
                u.Id,
                u.Username,
                u.Email,
                u.Phone,
                u.RealName,
                u.CreatedAt,
                u.AssignedRoleIds.ToList()
            ));

        return await usersQuery.ToPagedDataAsync(request.PageRequest, cancellationToken);
    }
}