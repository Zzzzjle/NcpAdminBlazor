using FastEndpoints;
using NcpAdminBlazor.Shared.Auth;

namespace NcpAdminBlazor.ApiService.Endpoints.RolesManagement;

public sealed class AllPermissionsEndpoint : EndpointWithoutRequest<ResponseData<AllPermissionsResponse>>
{
    public override void Configure()
    {
        Get("/api/permissions");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var allGroups = AppPermissions.GetAllGroups();
        var groupDtos = allGroups.Select(MapToDto).ToList();
        var response = new AllPermissionsResponse(groupDtos);
        await Send.OkAsync(response.AsResponseData(), ct);
    }

    private static PermissionGroupDto MapToDto(PermissionGroupInfo group)
    {
        return new PermissionGroupDto(
            group.Key,
            group.LogicalName,
            group.DisplayName,
            group.Description,
            group.ParentKey,
            group.SubGroups.Select(MapToDto).ToList(),
            group.Permissions.Select(p => new PermissionItemDto(
                p.Key,
                p.LogicalName,
                p.DisplayName,
                p.Description,
                p.GroupKey
            )).ToList()
        );
    }
}

public sealed record AllPermissionsResponse(List<PermissionGroupDto> Groups);

public sealed record PermissionGroupDto(
    string Key,
    string LogicalName,
    string DisplayName,
    string Description,
    string? ParentKey,
    List<PermissionGroupDto> SubGroups,
    List<PermissionItemDto> Permissions
);

public sealed record PermissionItemDto(
    string Key,
    string LogicalName,
    string DisplayName,
    string Description,
    string GroupKey
);