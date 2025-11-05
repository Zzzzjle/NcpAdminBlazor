namespace NcpAdminBlazor.Client.Pages.Applications.Role.Models;

public class RoleSearchFilter
{
    public string? Name { get; set; }
}

public class RoleFormModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsDisable { get; set; } = false;
}