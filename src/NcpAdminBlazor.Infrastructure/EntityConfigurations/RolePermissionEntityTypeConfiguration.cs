using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal class RolePermissionEntityTypeConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseSnowFlakeValueGenerator();
        
        builder.Property(t => t.PermissionCode).HasMaxLength(100).IsRequired();
        
        // 索引配置
                builder.HasIndex(rp => new { rp.RoleId, rp.PermissionCode }).IsUnique();
    }
}