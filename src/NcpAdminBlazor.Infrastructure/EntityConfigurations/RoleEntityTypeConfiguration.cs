using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal sealed class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("角色标识");

        builder.Property(role => role.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("角色名称");

        builder.Property(role => role.Description)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("角色描述");

        builder.Property(role => role.IsDisabled)
            .IsRequired()
            .HasComment("是否禁用");

        builder.Property(role => role.CreatedAt)
            .IsRequired()
            .HasComment("创建时间");

        builder.Property(role => role.AssignedMenuIds)
            .HasConversion(
                v => string.Join(',', v.Select(id => id.Id.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => new MenuId(Guid.Parse(s)))
                    .ToList())
            .HasColumnType("text")
            .HasComment("分配的菜单ID(逗号分隔)");

        builder.Property(role => role.AssignedPermissionCodes)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnType("text")
            .HasComment("分配的权限代码(逗号分隔)");

        builder.Property(role => role.IsDeleted)
            .IsRequired()
            .HasComment("是否已删除");

        builder.Property(role => role.DeletedAt)
            .IsRequired()
            .HasComment("删除时间");

        builder.HasIndex(role => role.Name)
            .IsUnique();
    }
}