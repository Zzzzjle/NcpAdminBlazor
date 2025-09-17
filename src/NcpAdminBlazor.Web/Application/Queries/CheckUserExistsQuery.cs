using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Infrastructure;

namespace NcpAdminBlazor.Web.Application.Queries;

public record CheckUserExistsQuery(string? Name, string? Email, string? Phone) : IQuery<UserExistsDto>;

public record UserExistsDto(bool NameExists, bool EmailExists, bool PhoneExists);

public class CheckUserExistsQueryValidator : AbstractValidator<CheckUserExistsQuery>
{
    public CheckUserExistsQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.Phone))
            .WithMessage("至少提供一个检查条件");
    }
}

public class CheckUserExistsQueryHandler(ApplicationDbContext context) 
    : IQueryHandler<CheckUserExistsQuery, UserExistsDto>
{
    public async Task<UserExistsDto> Handle(CheckUserExistsQuery request, CancellationToken cancellationToken)
    {
        var nameExists = false;
        var emailExists = false;
        var phoneExists = false;
        
        if (!string.IsNullOrEmpty(request.Name))
        {
            nameExists = await context.ApplicationUsers
                .AnyAsync(u => u.Name == request.Name && !u.IsDeleted, cancellationToken);
        }
        
        if (!string.IsNullOrEmpty(request.Email))
        {
            emailExists = await context.ApplicationUsers
                .AnyAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);
        }
        
        if (!string.IsNullOrEmpty(request.Phone))
        {
            phoneExists = await context.ApplicationUsers
                .AnyAsync(u => u.Phone == request.Phone && !u.IsDeleted, cancellationToken);
        }
        
        return new UserExistsDto(nameExists, emailExists, phoneExists);
    }
}