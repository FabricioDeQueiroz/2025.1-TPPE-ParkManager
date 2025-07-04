using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ParkManager_Service.Data;
using ParkManager_Service.Helpers;
using ParkManager_Service.Models;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Services
{
    public class EstacionamentoService(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IEstacionamento
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        private string? GetUserId() => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private bool IsGerente() => _httpContextAccessor.HttpContext?.User.IsInRole("Gerente") ?? false;

        public async Task<IEnumerable<EstacionamentoGetDto>> GetAllEstacionamentosAsync()
        {
            string? userId = GetUserId();

            var query = db.Estacionamentos.AsQueryable();

            if (IsGerente())
            {
                query = query.Where(e => e.IdGerente == userId);
            }

            var estacionamentos = await query.ToListAsync().ConfigureAwait(false);

            return estacionamentos.Select(e => new EstacionamentoGetDto
            {
                IdEstacionamento = e.IdEstacionamento,
                Nome = e.Nome,
                NomeContratante = e.NomeContratante,
                VagasTotais = e.VagasTotais,
                VagasOcupadas = e.VagasOcupadas,
                Faturamento = e.Faturamento,
                RetornoContratante = e.RetornoContratante,
                ValorFracao = e.ValorFracao,
                DescontoHora = e.DescontoHora,
                ValorMensal = e.ValorMensal,
                ValorDiaria = e.ValorDiaria,
                AdicionalNoturno = e.AdicionalNoturno,
                HoraAbertura = e.HoraAbertura,
                HoraFechamento = e.HoraFechamento,
                Tipo = e.Tipo,
                IdGerente = e.IdGerente
            });
        }

        public async Task<EstacionamentoGetDto?> GetEstacionamentoByIdAsync(int id)
        {
            var estacionamento = await db.Estacionamentos.FirstOrDefaultAsync(e => e.IdEstacionamento == id).ConfigureAwait(false);

            if (estacionamento == null) return null;

            if (IsGerente() && estacionamento.IdGerente != GetUserId()) return null;

            return new EstacionamentoGetDto
            {
                IdEstacionamento = estacionamento.IdEstacionamento,
                Nome = estacionamento.Nome,
                NomeContratante = estacionamento.NomeContratante,
                VagasTotais = estacionamento.VagasTotais,
                VagasOcupadas = estacionamento.VagasOcupadas,
                Faturamento = estacionamento.Faturamento,
                RetornoContratante = estacionamento.RetornoContratante,
                ValorFracao = estacionamento.ValorFracao,
                DescontoHora = estacionamento.DescontoHora,
                ValorMensal = estacionamento.ValorMensal,
                ValorDiaria = estacionamento.ValorDiaria,
                AdicionalNoturno = estacionamento.AdicionalNoturno,
                HoraAbertura = estacionamento.HoraAbertura,
                HoraFechamento = estacionamento.HoraFechamento,
                Tipo = estacionamento.Tipo,
                IdGerente = estacionamento.IdGerente
            };
        }

        public async Task<Resultado<EstacionamentoGetDto>> AddEstacionamentoAsync(Estacionamento estacionamento)
        {
            string? userId = GetUserId();

            if (userId == null) return Resultado<EstacionamentoGetDto>.Falha("Usuário não autenticado.");

            estacionamento.IdGerente = userId;

            db.Estacionamentos.Add(estacionamento);

            await db.SaveChangesAsync()
                .ConfigureAwait(false);

            return Resultado<EstacionamentoGetDto>.Ok(
                new EstacionamentoGetDto
                {
                    IdEstacionamento = estacionamento.IdEstacionamento,
                    Nome = estacionamento.Nome,
                    NomeContratante = estacionamento.NomeContratante,
                    VagasTotais = estacionamento.VagasTotais,
                    VagasOcupadas = estacionamento.VagasOcupadas,
                    Faturamento = estacionamento.Faturamento,
                    RetornoContratante = estacionamento.RetornoContratante,
                    ValorFracao = estacionamento.ValorFracao,
                    DescontoHora = estacionamento.DescontoHora,
                    ValorMensal = estacionamento.ValorMensal,
                    ValorDiaria = estacionamento.ValorDiaria,
                    AdicionalNoturno = estacionamento.AdicionalNoturno,
                    HoraAbertura = estacionamento.HoraAbertura,
                    HoraFechamento = estacionamento.HoraFechamento,
                    Tipo = estacionamento.Tipo,
                    IdGerente = estacionamento.IdGerente
                }
            );
        }

        public async Task<bool> UpdateEstacionamentoAsync(Estacionamento estacionamento)
        {
            var existente = await db.Estacionamentos.AsNoTracking().FirstOrDefaultAsync(e => e.IdEstacionamento == estacionamento.IdEstacionamento).ConfigureAwait(false);

            if (existente == null || existente.IdGerente != GetUserId()) return false;

            db.Entry(estacionamento).State = EntityState.Modified;

            await db.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        public async Task<bool> DeleteEstacionamentoAsync(int id)
        {
            var estacionamento = await db.Estacionamentos.FindAsync(id).ConfigureAwait(false);

            if (estacionamento == null || estacionamento.IdGerente != GetUserId()) return false;

            db.Estacionamentos.Remove(estacionamento);

            await db.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }
    }
}
