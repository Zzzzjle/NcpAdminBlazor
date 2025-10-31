using NcpAdminBlazor.Domain.AggregatesModel.DeliverAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NcpAdminBlazor.Infrastructure.EntityConfigurations;

internal class DeliverRecordConfiguration : IEntityTypeConfiguration<DeliverRecord>
{
    public void Configure(EntityTypeBuilder<DeliverRecord> builder)
    {
        builder.ToTable("deliver_record");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseSnowFlakeValueGenerator();
    }
}