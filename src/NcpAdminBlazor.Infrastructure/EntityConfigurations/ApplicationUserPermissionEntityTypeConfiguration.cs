using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal class ApplicationUserPermissionEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUserPermission>
{
    public void Configure(EntityTypeBuilder<ApplicationUserPermission> builder)
    {
        builder.ToTable("application_user_permissions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseSnowFlakeValueGenerator();
        
        builder.Property(t => t.PermissionCode).HasMaxLength(100).IsRequired();
        
        // 外键约束
        builder.Property<ApplicationUserId>("ApplicationUserId").IsRequired();
        
        // 配置SourceRoleIds作为JSON列存储
        builder.Property(t => t.SourceRoleIds)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<HashSet<RoleId>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new HashSet<RoleId>()
            )
            .HasColumnType("text");
        
        // 索引配置
        builder.HasIndex("ApplicationUserId", "PermissionCode").IsUnique();
    }
}