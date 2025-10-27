namespace NcpAdminBlazor.Client.Pages.Applications.Role.Models;

public class RoleSearchFilter
{
    public string? Name { get; set; }

    public int? Status { get; set; }
}

public class RoleFormModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Status { get; set; } = 1;

    public IEnumerable<string> PermissionCodes { get; set; } = [];
}