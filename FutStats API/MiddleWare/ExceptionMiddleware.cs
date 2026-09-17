using FutStatsAPI.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace FutStatsAPI.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await EscreverResposta(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (BusinessException ex)
            {
                await EscreverResposta(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                // Qualquer erro não esperado (ex: falha de conexão com o Oracle) vira 500,
                // mas logamos o detalhe completo pra investigação — sem expor isso ao cliente
                _logger.LogError(ex, "Erro não tratado na requisição {Path}", context.Request.Path);
                await EscreverResposta(context, HttpStatusCode.InternalServerError, "Ocorreu um erro interno. Tente novamente mais tarde.");
            }
        }

        private static async Task EscreverResposta(HttpContext context, HttpStatusCode statusCode, string mensagem)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var resposta = JsonSerializer.Serialize(new { erro = mensagem });
            await context.Response.WriteAsync(resposta);
        }
    }
}