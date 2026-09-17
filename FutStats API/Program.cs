using System.Threading.RateLimiting;
using FutStatsAPI.API.Middlewares;
using FutStatsAPI.Application.Interfaces;
using FutStatsAPI.Application.Mappings;
using FutStatsAPI.Application.Services;
using FutStatsAPI.Domain.Interfaces;
using FutStatsAPI.Infrastructure.Data;
using FutStatsAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// ===== 1) Controllers =====
builder.Services.AddControllers();

// ===== 2) Banco de dados (Oracle via EF Core) =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString));

// ===== 3) Injeção de dependência: Repositories e Services =====
builder.Services.AddScoped<ITimeRepository, TimeRepository>();
builder.Services.AddScoped<IJogadorRepository, JogadorRepository>();
builder.Services.AddScoped<ITimeService, TimeService>();
builder.Services.AddScoped<IJogadorService, JogadorService>();

// ===== 4) AutoMapper =====
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ===== 5) Swagger com Annotations =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FutStats API",
        Version = "v1",
        Description = "API para gerenciamento de times e jogadores de futebol — CP4 Advanced Business Development"
    });
    options.EnableAnnotations();
});

// ===== 6) Compressão de resposta (Gzip + Brotli) =====
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

// ===== 7) Rate Limiting (limite por IP) =====
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";

        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,               // até 10 requisições
            Window = TimeSpan.FromSeconds(10), // a cada 10 segundos
            QueueLimit = 0                  // sem fila: excedeu, recebe 429 na hora
        });
    });
});

// ===== 8) Health Checks =====
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(
        name: "oracle-database",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "db", "oracle" });

var app = builder.Build();

// ===== Pipeline HTTP =====

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FutStats API v1");
    });
}

app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
public partial class Program { }