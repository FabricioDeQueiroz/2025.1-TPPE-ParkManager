using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using ParkManager_Service.Data;
using ParkManager_Service.Services;
using ParkManager_Service.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Limita os logs durante o Teste para não ficar poluído:
if (builder.Environment.IsEnvironment("Test"))
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.None);
    builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.None);
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ParkManager - Service", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IEstacionamento, EstacionamentoService>();

var app = builder.Build();

// Para que o Swagger funcione também no ambiente de produção
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ParkManager v1");
    c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
});
app.UseStaticFiles();

// if (app.Environment.IsDevelopment())
// {
// }

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Para ser visível nos Testes
public abstract partial class Program { }
