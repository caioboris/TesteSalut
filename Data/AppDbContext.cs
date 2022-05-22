using Microsoft.EntityFrameworkCore;
using TesteSalut.Models;

namespace TesteSalut.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options ): base(options) {}

        public DbSet<Produto> Produto { get; set; }

        public DbSet<NotaFiscal> NotaFiscal { get; set; }

        public DbSet<CupomFiscal> CupomFiscal { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-8OK7LBP\\SQLEXPRESS;Database=TesteSalutDB;Trusted_Connection=True;MultipleActiveResultSets=true");

    }
}
