using System;

namespace FutStatsAPI.Domain.Entities
{
    public class Jogador
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Numero { get; set; }
        public PosicaoJogador Posicao { get; set; }
        public DateTime DataNascimento { get; set; }

        // Chave estrangeira
        public int TimeId { get; set; }
        public Time? Time { get; set; }
    }
}