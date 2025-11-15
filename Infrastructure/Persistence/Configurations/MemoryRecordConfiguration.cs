using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations {
    public class MemoryRecordConfiguration : IEntityTypeConfiguration<MemoryRecord>{
        public void Configure(EntityTypeBuilder<MemoryRecord> builder) {
            builder.ToTable("memory_records");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(m => m.Description)
                .HasColumnName("description")
                .HasMaxLength(300);

            builder.HasIndex(m => m.CreatedAt);

            builder.Property(m => m.ChainStateJson)
                .HasColumnName("chain_state_json")
                .IsRequired();
        }
    }
}
