using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FutStatsAPI.Application.DTOs
{
    // O que a API retorna ao listar/buscar um time (sem expor a entidade do banco)
    public class TimeDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int AnoFundacao { get; set; }
        public int QuantidadeJogadores { get; set; }
    }

    // Usado dentro do TimeDetalheDto (não repete todos os campos do jogador, só o essencial)
    public class JogadorResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Numero { get; set; }
        public string Posicao { get; set; } = string.Empty;
    }

    // Time com a lista de jogadores, usado no GET por Id
    public class TimeDetalheDto : TimeDto
    {
        public List<JogadorResumoDto> Jogadores { get; set; } = new();
    }

    // O que o cliente ENVIA para criar um time
    public class CriarTimeDto
    {
        [Required(ErrorMessage = "O nome do time é obrigatório")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [Required]
        [MaxLength(2, ErrorMessage = "Use a sigla do estado, ex: SP")]
        public string Estado { get; set; } = string.Empty;

        [Range(1800, 2026, ErrorMessage = "Ano de fundação inválido")]
        public int AnoFundacao { get; set; }
        
    }

    // O que o cliente ENVIA para atualizar um time
    public class AtualizarTimeDto
    {
        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [Required, MaxLength(2)]
        public string Estado { get; set; } = string.Empty;

        [Range(1800, 2026)]
        public int AnoFundacao { get; set; }
        
    }
}