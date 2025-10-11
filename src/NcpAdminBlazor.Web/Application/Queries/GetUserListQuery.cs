using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Web.Application.Queries;

public record GetUserListQuery(
    string? Username,
    string? Email,
    string? Phone,
    string? RealName,
    int PageIndex = 1,
    int PageSize = 10,
    bool CountTotal = true
) : IQuery<PagedData<UserListItemDto>>;

public record UserListItemDto(
    ApplicationUserId Id,
    string Username,
    string Email,
    string Phone,
    string RealName,
    int Status,
    DateTimeOffset CreatedAt,
    List<string> RoleNames
);

public class GetUserListQueryValidator : AbstractValidator<GetUserListQuery>
{
    public GetUserListQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("页码必须大于0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("每页条数必须在1-100之间");
    }
}

public class GetUserListQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetUserListQuery, PagedData<UserListItemDto>>
{
    public async Task<PagedData<UserListItemDto>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        var query = context.ApplicationUsers
            .Include(u => u.Roles)
            .WhereIf(!string.IsNullOrEmpty(request.Username), u => u.Username.Contains(request.Username!))
            .WhereIf(!string.IsNullOrEmpty(request.Email), u => u.Email.Contains(request.Email!))
            .WhereIf(!string.IsNullOrEmpty(request.Phone), u => u.Phone.Contains(request.Phone!))
            .WhereIf(!string.IsNullOrEmpty(request.RealName), u => u.RealName.Contains(request.RealName!))
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserListItemDto(
                u.Id,
                u.Username,
                u.Email,
                u.Phone,
                u.RealName,
                u.Status,
                u.CreatedAt,
                u.Roles.Select(r => r.RoleName).ToList()
            ));

        return await query.ToPagedDataAsync(request.PageIndex, request.PageSize, request.CountTotal, cancellationToken: cancellationToken);
    }
}