using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using ParkManager_Service.Data;
using ParkManager_Service.Services;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Limita os logs durante o Teste para não ficar poluído:
if (builder.Environment.IsEnvironment("Test"))
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.None);
    builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.None);
    builder.Logging.AddFilter("Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware", LogLevel.None);
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ParkManager - Service", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT:"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IEstacionamento, EstacionamentoService>();
builder.Services.AddScoped<IUsuario, UsuarioService>();

builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Para ser visível nos Testes
public abstract partial class Program { }
