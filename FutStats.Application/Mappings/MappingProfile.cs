using AutoMapper;
using FutStatsAPI.Application.DTOs;
using FutStatsAPI.Domain.Entities;

namespace FutStatsAPI.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Time -> TimeDto
            CreateMap<Time, TimeDto>()
                .ForMember(dest => dest.QuantidadeJogadores,
                    opt => opt.MapFrom(src => src.Jogadores.Count));

            // Time -> TimeDetalheDto (herda de TimeDto, então reaproveita o mapeamento)
            CreateMap<Time, TimeDetalheDto>()
                .ForMember(dest => dest.QuantidadeJogadores,
                    opt => opt.MapFrom(src => src.Jogadores.Count));

            // CriarTimeDto -> Time (entrada do cliente vira entidade)
            CreateMap<CriarTimeDto, Time>();

            // Jogador -> JogadorResumoDto (usado dentro do TimeDetalheDto)
            CreateMap<Jogador, JogadorResumoDto>()
                .ForMember(dest => dest.Posicao,
                    opt => opt.MapFrom(src => src.Posicao.ToString()));

            // Jogador -> JogadorDto
            CreateMap<Jogador, JogadorDto>()
                .ForMember(dest => dest.Posicao,
                    opt => opt.MapFrom(src => src.Posicao.ToString()))
                .ForMember(dest => dest.TimeNome,
                    opt => opt.MapFrom(src => src.Time != null ? src.Time.Nome : string.Empty));
        }
    }
}