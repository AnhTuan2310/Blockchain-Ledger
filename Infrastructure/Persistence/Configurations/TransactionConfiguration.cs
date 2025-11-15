using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction> {
    public void Configure(EntityTypeBuilder<Transaction> builder) {
        builder.ToTable("transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(t => t.Content)
            .HasColumnName("content")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.Author)
            .HasColumnName("author")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();

        builder.Property(t => t.BlockHash)
            .HasColumnName("block_hash")   // <= make it explicit
            .IsRequired(false);

        builder.HasOne(t => t.Block)
            .WithMany(b => b.Transactions)
            .HasForeignKey(t => t.BlockHash)   // <= CLR property, not a string
            .HasPrincipalKey(b => b.Hash)      // keep only if Block.Hash isn't the PK
            .OnDelete(DeleteBehavior.SetNull);
    }
}
