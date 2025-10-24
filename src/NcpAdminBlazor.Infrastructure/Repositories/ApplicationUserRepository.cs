using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Infrastructure.Repositories;

public interface IApplicationUserRepository : IRepository<ApplicationUser, ApplicationUserId>
{
    /// <summary>
    /// 根据邮箱获取用户
    /// </summary>
    /// <param name="email">邮箱地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户实体，如果不存在则返回null</returns>
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    /// <param name="name">用户名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户实体，如果不存在则返回null</returns>
    Task<ApplicationUser?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据手机号获取用户
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户实体，如果不存在则返回null</returns>
    Task<ApplicationUser?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据角色ID获取拥有该角色的用户列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户实体列表</returns>
    Task<List<ApplicationUser>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default);
}

public class ApplicationUserRepository(ApplicationDbContext context) : RepositoryBase<ApplicationUser, ApplicationUserId, ApplicationDbContext>(context), IApplicationUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationUsers
            .Include(u => u.Roles)
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted, cancellationToken);
    }
    
    public async Task<ApplicationUser?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationUsers
            .Include(u => u.Roles)
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(x => x.Username == name && !x.IsDeleted, cancellationToken);
    }
    
    public async Task<ApplicationUser?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationUsers
            .Include(u => u.Roles)
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(x => x.Phone == phone && !x.IsDeleted, cancellationToken);
    }

    public async Task<List<ApplicationUser>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationUsers
            .Include(u => u.Roles)
            .Include(u => u.Permissions)
            .Where(u => !u.IsDeleted && u.Roles.Any(r => r.RoleId == roleId))
            .ToListAsync(cancellationToken);
    }
}