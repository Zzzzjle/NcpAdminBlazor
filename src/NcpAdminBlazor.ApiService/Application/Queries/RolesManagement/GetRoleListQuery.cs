using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.RolesManagement;

public record GetRoleListQuery(
    string? Name,
    IPageRequest PageRequest) :
    IQuery<PagedData<RoleListItemDto>>;

public record RoleListItemDto(
    RoleId RoleId,
    string Name,
    string Description,
    bool IsDisabled,
    DateTimeOffset CreatedAt);

public class GetRoleListQueryValidator : AbstractValidator<GetRoleListQuery>
{
    public GetRoleListQueryValidator()
    {
        RuleFor(x => x.PageRequest.PageIndex)
            .GreaterThan(0).WithMessage("页码必须大于0");

        RuleFor(x => x.PageRequest.PageSize)
            .InclusiveBetween(1, 100).WithMessage("每页条数必须在1-100之间");
    }
}

public class GetRoleListQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetRoleListQuery, PagedData<RoleListItemDto>>
{
    public async Task<PagedData<RoleListItemDto>> Handle(GetRoleListQuery request, CancellationToken cancellationToken)
    {
        var queryable = context.Roles
            .WhereIf(!string.IsNullOrEmpty(request.Name), r => r.Name.Contains(request.Name!))
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RoleListItemDto(
                r.Id,
                r.Name,
                r.Description,
                r.IsDisabled,
                r.CreatedAt));

        var result = await queryable.ToPagedDataAsync(request.PageRequest,
            cancellationToken);
        return result;
    }
}