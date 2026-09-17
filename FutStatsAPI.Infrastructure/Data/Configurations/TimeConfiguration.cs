using FutStatsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FutStatsAPI.Infrastructure.Data.Configurations
{
    public class TimeConfiguration : IEntityTypeConfiguration<Time>
    {
        public void Configure(EntityTypeBuilder<Time> builder)
        {
            builder.ToTable("TIMES");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Estado)
                .IsRequired()
                .HasMaxLength(2);

            // Índice único: garante no banco que não existam dois times com o mesmo nome,
            // reforçando a regra que já validamos no TimeService
            builder.HasIndex(t => t.Nome)
                .IsUnique()
                .HasDatabaseName("IX_TIMES_NOME");

            builder.HasMany(t => t.Jogadores)
                .WithOne(j => j.Time)
                .HasForeignKey(j => j.TimeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}