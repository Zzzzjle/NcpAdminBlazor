using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Infrastructure.Repositories;

public interface IMenuRepository : IRepository<Menu, MenuId>;

public sealed class MenuRepository(ApplicationDbContext context)
    : RepositoryBase<Menu, MenuId, ApplicationDbContext>(context), IMenuRepository;