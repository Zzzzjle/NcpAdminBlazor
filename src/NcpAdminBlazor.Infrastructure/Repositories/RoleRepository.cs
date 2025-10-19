using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Infrastructure.Repositories;

public interface IRoleRepository : IRepository<Role, RoleId>
{
    /// <summary>
    /// 根据角色名称获取角色
    /// </summary>
    /// <param name="name">角色名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色实体，如果不存在则返回null</returns>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据角色ID列表获取角色
    /// </summary>
    /// <param name="roleIds">角色ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色实体列表</returns>
    Task<List<Role>> GetByIdsAsync(IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default);
}

public class RoleRepository(ApplicationDbContext context) : RepositoryBase<Role, RoleId, ApplicationDbContext>(context), IRoleRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
    
    public async Task<List<Role>> GetByIdsAsync(IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }
}