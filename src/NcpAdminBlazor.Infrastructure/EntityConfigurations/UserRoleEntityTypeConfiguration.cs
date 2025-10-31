using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("application_user_roles");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseSnowFlakeValueGenerator();
        
        builder.Property(t => t.RoleId).IsRequired();
        builder.Property(t => t.RoleName).HasMaxLength(50).IsRequired();
        
        // 外键约束
        builder.Property<ApplicationUserId>("UserId").IsRequired();
        
        // 索引配置
        builder.HasIndex("UserId", "RoleId").IsUnique();
    }
}