using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Web.AspNetCore;

public interface ICurrentUser
{
    ApplicationUserId? UserId { get; }
    string UserName { get; }
}

public class CurrentUser : ICurrentUser
{
    public ApplicationUserId? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}