using System;
using System.ComponentModel.DataAnnotations;

namespace FutStatsAPI.Application.DTOs
{
    // O que a API retorna ao listar/buscar um jogador
    public class JogadorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Numero { get; set; }
        public string Posicao { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public int TimeId { get; set; }
        public string TimeNome { get; set; } = string.Empty;
    }

    // O que o cliente ENVIA para criar um jogador
    public class CriarJogadorDto
    {
        [Required(ErrorMessage = "O nome do jogador é obrigatório")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Range(1, 99, ErrorMessage = "Número da camisa deve ser entre 1 e 99")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "Informe a posição do jogador")]
        public string Posicao { get; set; } = string.Empty;

        [Required]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "É obrigatório vincular o jogador a um time")]
        public int TimeId { get; set; }
    }

    // O que o cliente ENVIA para atualizar um jogador
    public class AtualizarJogadorDto
    {
        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Range(1, 99)]
        public int Numero { get; set; }

        [Required]
        public string Posicao { get; set; } = string.Empty;

        [Required]
        public DateTime DataNascimento { get; set; }
    }
}