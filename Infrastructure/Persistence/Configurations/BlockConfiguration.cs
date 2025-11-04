using Domain.Entities;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations {
    public class BlockConfiguration : IEntityTypeConfiguration<Block>{
        public void Configure(EntityTypeBuilder<Block> builder) {
            builder.ToTable("blocks");
            builder.HasKey(b => b.Hash);

            builder.Property(b => b.Hash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(b => b.PreviousHash)
                .IsRequired()
                .HasMaxLength(256);
            builder.Property(b => b.MinerNote)
                .HasMaxLength(512);

            builder.Property(b => b.Data)
                .IsRequired(false);


            builder.HasMany(b => b.Transactions)
                .WithOne()
                .HasForeignKey("block_hash")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
