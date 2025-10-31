using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal sealed class MenuEntityTypeConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("menus");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .UseGuidVersion7ValueGenerator();

        builder.Property(x => x.ParentId);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Icon)
            .HasMaxLength(64);

        builder.Property(x => x.PageKey)
            .HasMaxLength(128);

        builder.Property(x => x.Path)
            .HasMaxLength(256);

        builder.Property(x => x.PermissionCode)
            .HasMaxLength(128);

        builder.Property(x => x.Type)
            .HasConversion<int>();

        builder.Property(x => x.Order)
            .IsRequired();

        builder.Property(x => x.IsDisable)
            .IsRequired();
    }
}