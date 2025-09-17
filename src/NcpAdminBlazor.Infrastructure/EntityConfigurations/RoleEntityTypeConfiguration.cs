using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseSnowFlakeValueGenerator();
        
        builder.Property(t => t.Name).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Status).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();
        
        // 索引配置
        builder.HasIndex(t => t.Name).IsUnique();
        
        // 配置与权限的关系
        builder.HasMany(r => r.Permissions)
            .WithOne()
            .HasForeignKey("RoleId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}