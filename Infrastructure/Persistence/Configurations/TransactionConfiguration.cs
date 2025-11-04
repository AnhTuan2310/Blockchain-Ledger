using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configurations {
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>{
        public void Configure(EntityTypeBuilder<Transaction> builder) {
            builder.ToTable("transactions");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(t => t.Content)
                .HasColumnName("content")
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.Timestamp)
                .HasColumnName("timestamp")
                .IsRequired();

            builder.Property<string>("block_hash")
                .HasColumnName("block_hash")
                .IsRequired(); 
        }
    }
}
