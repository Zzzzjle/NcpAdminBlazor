using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal class ApplicationUserRoleEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.ToTable("application_user_roles");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseSnowFlakeValueGenerator();
        
        builder.Property(t => t.RoleId).IsRequired();
        builder.Property(t => t.RoleName).HasMaxLength(50).IsRequired();
        
        // 外键约束
        builder.Property<ApplicationUserId>("ApplicationUserId").IsRequired();
        
        // 索引配置
        builder.HasIndex("ApplicationUserId", "RoleId").IsUnique();
    }
}