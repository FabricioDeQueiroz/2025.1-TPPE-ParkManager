using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using ParkManager_Service.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ParkManager - Service", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

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
