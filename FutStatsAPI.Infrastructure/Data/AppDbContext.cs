using FutStatsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FutStatsAPI.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Time> Times => Set<Time>();
        public DbSet<Jogador> Jogadores => Set<Jogador>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica todas as classes IEntityTypeConfiguration<> deste assembly automaticamente
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}