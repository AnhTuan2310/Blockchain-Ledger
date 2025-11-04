using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence {
    public class AppDbContext : DbContext {
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<MemoryRecord> MemoryRecords { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContext).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.UseSnakeCaseConvention();


        }
    }
}
