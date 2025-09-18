namespace NcpAdminBlazor.Web.AspNetCore;

public interface ICurrentUser
{
    long UserId { get; }
    string UserName { get; }
}

public class CurrentUser : ICurrentUser
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}