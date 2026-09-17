using System.Collections.Generic;

namespace FutStatsAPI.Domain.Entities
{
    public class Time
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int AnoFundacao { get; set; }

        public ICollection<Jogador> Jogadores { get; set; } = new List<Jogador>();
    }
}