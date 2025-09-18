using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("application_users");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseSnowFlakeValueGenerator();
        
        builder.Property(t => t.Username).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Phone).HasMaxLength(20).IsRequired();
        builder.Property(t => t.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(t => t.PasswordSalt).HasMaxLength(256).IsRequired();
        builder.Property(t => t.RealName).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Email).HasMaxLength(100).IsRequired();
    builder.Property(t => t.RefreshToken).HasMaxLength(256);
    builder.Property(t => t.RefreshExpiry);
        builder.Property(t => t.Status).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.IsDeleted).IsRequired();
        builder.Property(t => t.DeletedAt);
        
        // 索引配置
        builder.HasIndex(t => t.Username).IsUnique().HasFilter("IsDeleted = 0");
        builder.HasIndex(t => t.Email).IsUnique().HasFilter("IsDeleted = 0");
        builder.HasIndex(t => t.Phone).IsUnique().HasFilter("IsDeleted = 0");
        
        // 配置与角色的关系
        builder.HasMany(u => u.Roles)
            .WithOne()
            .HasForeignKey("ApplicationUserId")
            .OnDelete(DeleteBehavior.Cascade);
            
        // 配置与权限的关系
        builder.HasMany(u => u.Permissions)
            .WithOne()
            .HasForeignKey("ApplicationUserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}