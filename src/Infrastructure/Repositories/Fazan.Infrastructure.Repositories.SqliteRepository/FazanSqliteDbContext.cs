namespace Fazan.Infrastructure.Repositories.SqliteRepository
{
    using Domain.Models;
    using Microsoft.EntityFrameworkCore;

    public sealed class FazanSqliteDbContext: DbContext
    {
        public FazanSqliteDbContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Word> Words { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Word>().ToTable("Words").HasKey(entity => entity.Id);
            modelBuilder.Entity<Word>().HasIndex(entity => entity.Value).IsUnique(true);
            modelBuilder.Entity<Word>().HasIndex(entity => entity.FirstLetters);
            modelBuilder.Entity<Word>().HasIndex(entity => entity.LastLetters);
            modelBuilder.Entity<Word>().HasIndex(entity => entity.DerivedWordsCount);
        }
    }
}
