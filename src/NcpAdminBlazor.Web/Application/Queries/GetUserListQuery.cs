using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Infrastructure;
using NetCorePal.Extensions.AspNetCore;

namespace NcpAdminBlazor.Web.Application.Queries;

public record GetUserListQuery(
    string? Name,
    string? Email,
    int? Status,
    int PageIndex = 1,
    int PageSize = 10
) : IQuery<PagedData<UserListItemDto>>;

public record UserListItemDto(
    string Id,
    string Name,
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
            .Where(u => !u.IsDeleted)
            .WhereIf(!string.IsNullOrEmpty(request.Name), u => u.Name.Contains(request.Name!))
            .WhereIf(!string.IsNullOrEmpty(request.Email), u => u.Email.Contains(request.Email!))
            .WhereIf(request.Status.HasValue, u => u.Status == request.Status!.Value)
            .OrderBy(u => u.CreatedAt)
            .Select(u => new UserListItemDto(
                u.Id.ToString(),
                u.Name,
                u.Email,
                u.Phone,
                u.RealName,
                u.Status,
                u.CreatedAt,
                u.Roles.Select(r => r.RoleName).ToList()
            ));

        return await query.ToPagedDataAsync(request.PageIndex, request.PageSize, cancellationToken: cancellationToken);
    }
}