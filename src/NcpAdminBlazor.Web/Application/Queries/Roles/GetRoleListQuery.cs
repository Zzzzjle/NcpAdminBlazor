using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.Roles;

public record GetRoleListQuery(
    string? Name,
    int? Status,
    int PageIndex,
    int PageSize,
    bool CountTotal) : IPagedQuery<RoleListItemDto>;

public record RoleListItemDto(
    RoleId RoleId,
    string Name,
    string Description,
    int Status,
    DateTimeOffset CreatedAt);

public class GetRoleListQueryValidator : AbstractValidator<GetRoleListQuery>
{
    public GetRoleListQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("页码必须大于0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("每页条数必须在1-100之间");

        RuleFor(x => x.Status)
            .Must(status => status is null or 0 or 1)
            .WithMessage("角色状态必须是0或1");
    }
}

public class GetRoleListQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetRoleListQuery, PagedData<RoleListItemDto>>
{
    public async Task<PagedData<RoleListItemDto>> Handle(GetRoleListQuery request, CancellationToken cancellationToken)
    {
        var queryable = context.Roles
            .WhereIf(!string.IsNullOrEmpty(request.Name), r => r.Name.Contains(request.Name!))
            .WhereIf(request.Status.HasValue, r => r.Status == request.Status!.Value)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RoleListItemDto(
                r.Id,
                r.Name,
                r.Description,
                r.Status,
                r.CreatedAt));

        var result = await queryable.ToPagedDataAsync(
            request.PageIndex,
            request.PageSize,
            request.CountTotal,
            cancellationToken: cancellationToken);
        return result;
    }
}