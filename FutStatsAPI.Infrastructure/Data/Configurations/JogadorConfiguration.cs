using FutStatsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FutStatsAPI.Infrastructure.Data.Configurations
{
    public class JogadorConfiguration : IEntityTypeConfiguration<Jogador>
    {
        public void Configure(EntityTypeBuilder<Jogador> builder)
        {
            builder.ToTable("JOGADORES");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(j => j.Posicao)
                .IsRequired()
                .HasConversion<string>()   // salva o enum como texto no Oracle (ex: "Atacante"), mais legível que número
                .HasMaxLength(20);

            // Índice comum (não único) na FK: otimiza a consulta "jogadores de um time" (usada na paginação)
            builder.HasIndex(j => j.TimeId)
                .HasDatabaseName("IX_JOGADORES_TIMEID");
        }
    }
}