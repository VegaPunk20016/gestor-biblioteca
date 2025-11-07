using Microsoft.EntityFrameworkCore;
using ReadHub.Libro.Domain.Entities;

namespace ReadHub.Libro.Infrastructure.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();

        // ⬇️ AGREGA ESTE
        public DbSet<Loan> Loans => Set<Loan>();

        public DbSet<LoanItem> LoanItems => Set<LoanItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            modelBuilder.Entity<Loan>()
                .HasMany(l => l.Items)
                .WithOne(i => i.Loan)
                .HasForeignKey(i => i.LoanId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
