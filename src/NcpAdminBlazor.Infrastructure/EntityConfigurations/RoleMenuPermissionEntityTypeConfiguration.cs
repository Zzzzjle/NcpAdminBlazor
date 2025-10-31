using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal class RoleMenuPermissionEntityTypeConfiguration : IEntityTypeConfiguration<RoleMenuPermission>
{
    public void Configure(EntityTypeBuilder<RoleMenuPermission> builder)
    {
        builder.ToTable("role_menu_permissions");
        builder.HasKey(permission => permission.Id);
        builder.Property(permission => permission.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("角色菜单权限标识");

        builder.Property(permission => permission.RoleId)
            .IsRequired();

        builder.Property(permission => permission.PermissionCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(permission => permission.MenuId)
            .IsRequired();

        builder.HasIndex(permission => new { permission.RoleId, permission.PermissionCode })
            .IsUnique();
    }
}