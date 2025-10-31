using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal class UserMenuPermissionEntityTypeConfiguration : IEntityTypeConfiguration<UserMenuPermission>
{
    public void Configure(EntityTypeBuilder<UserMenuPermission> builder)
    {
        builder.ToTable("user_menu_permissions");
        builder.HasKey(permission => permission.Id);
        builder.Property(permission => permission.Id)
            .UseGuidVersion7ValueGenerator();

        builder.Property(permission => permission.UserId)
            .IsRequired();

        builder.Property(permission => permission.PermissionCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(permission => permission.MenuId)
            .IsRequired();

        builder.Property(permission => permission.SourceRoleId);
    }
}