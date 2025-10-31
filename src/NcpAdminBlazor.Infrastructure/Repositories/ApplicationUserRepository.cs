using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Infrastructure.Repositories;

public interface IApplicationUserRepository : IRepository<ApplicationUser, ApplicationUserId>;

public class ApplicationUserRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationUser, ApplicationUserId, ApplicationDbContext>(context), IApplicationUserRepository;