using System.Security.Claims;

namespace NcpAdminBlazor.Web.AspNetCore.Middlewares;


public class CurrentUserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
    {
        // 在此处，HttpContext.User 已经被认证管道完全填充
        var claimsPrincipal = context.User;

        // 确保注入的 currentUser 是 CurrentUser 类型，以便进行赋值
        if (currentUser is CurrentUser concreteCurrentUser)
        {
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                // 从 Claims 中提取用户信息并填充 CurrentUser
                var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
                var userNameClaim = claimsPrincipal.FindFirst(ClaimTypes.Name);

                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
                {
                    concreteCurrentUser.UserId = userId;
                }
                
                if (userNameClaim != null)
                {
                    concreteCurrentUser.UserName = userNameClaim.Value;
                }
            }
            else
            {
                // 匿名访问时，无需给CurrentUser赋值，或者设置为默认值
                concreteCurrentUser.UserId = 0;
                concreteCurrentUser.UserName = string.Empty; // 或者 "匿名用户"
            }
        }

        await next(context); // 继续请求管道
    }
}
