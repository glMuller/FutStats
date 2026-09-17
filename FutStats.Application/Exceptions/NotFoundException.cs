using System;

namespace FutStatsAPI.Application.Exceptions
{
    // Lançada quando um Time ou Jogador não é encontrado pelo Id
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}