using System;

namespace FutStatsAPI.Application.Exceptions
{
    // Lançada quando uma regra de negócio é violada (ex: nome de time duplicado)
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}