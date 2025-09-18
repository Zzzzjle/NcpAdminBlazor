namespace NcpAdminBlazor.Shared.EndpointsDtos.UserEndpoints;

/// <summary>
/// 获取用户信息请求模型
/// </summary>
public class GetUserInfoRequest
{
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// 用户信息响应模型
/// </summary>
public class UserInfoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<UserRoleDto> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}


/// <summary>
/// 用户角色DTO
/// </summary>
public class UserRoleDto
{
    public string RoleName { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
}
