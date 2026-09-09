using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Common.Infrastructure.Messaging;

namespace Modules.Catalog.Infrastructure.Database.Mapping;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
	public void Configure(EntityTypeBuilder<OutboxMessage> builder)
	{
		builder.ToTable("outbox_messages");
		builder.HasKey(message => message.Id);
		builder.Property(message => message.Type).IsRequired().HasMaxLength(200);
		builder.Property(message => message.Payload).IsRequired();
		builder.Property(message => message.OccurredOnUtc).IsRequired();
		builder.HasIndex(message => new { message.ProcessedOnUtc, message.OccurredOnUtc });
	}
}
