using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParkManager_Service.Models;

namespace ParkManager_Service.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<Usuario>(options)
    {
        public DbSet<Estacionamento> Estacionamentos { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Evento> Eventos { get; set; } = null!;
        public DbSet<Acesso> Acessos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // USUARIO (Gerente) - gerencia - ESTACIONAMENTO (1:N)
            builder.Entity<Estacionamento>()
                .HasOne(e => e.Gerente)
                .WithMany(u => u.EstacionamentosGerenciados)
                .HasForeignKey(e => e.IdGerente)
                .OnDelete(DeleteBehavior.Restrict);

            // ESTACIONAMENTO - possui - EVENTO (1:N)
            builder.Entity<Evento>()
                .HasOne(ev => ev.Estacionamento)
                .WithMany(e => e.Eventos)
                .HasForeignKey(ev => ev.IdEstacionamento)
                .OnDelete(DeleteBehavior.Cascade);

            // USUARIO (Cliente) - realiza - ACESSO (1:N)
            builder.Entity<Acesso>()
                .HasOne(a => a.Cliente)
                .WithMany(u => u.Acessos)
                .HasForeignKey(a => a.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            // ESTACIONAMENTO - registra - ACESSO (1:N)
            builder.Entity<Acesso>()
                .HasOne(a => a.Estacionamento)
                .WithMany(e => e.Acessos)
                .HasForeignKey(a => a.IdEstacionamento)
                .OnDelete(DeleteBehavior.Cascade);

            // ACESSO - em - EVENTO (N:1)
            builder.Entity<Acesso>()
                .HasOne(a => a.Evento)
                .WithMany(ev => ev.Acessos)
                .HasForeignKey(a => a.IdEvento)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(builder);
        }
    }
}
