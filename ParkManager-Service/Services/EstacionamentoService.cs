using Microsoft.EntityFrameworkCore;
using ParkManager_Service.Data;
using ParkManager_Service.Models;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Services
{
    public class EstacionamentoService(AppDbContext db) : IEstacionamento
    {
        public async Task<IEnumerable<EstacionamentoGetDto>> GetAllEstacionamentosAsync()
        {
            var estacionamentos = await db.Estacionamentos
                .ToListAsync()
                .ConfigureAwait(false);

            var estacionamentosDto = estacionamentos.Select(e => new EstacionamentoGetDto
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
                ValorEvento = e.ValorEvento,
                HoraAbertura = e.HoraAbertura,
                HoraFechamento = e.HoraFechamento,
                Tipo = e.Tipo,
                IdGerente = e.IdGerente
            }).ToList();

            return estacionamentosDto;
        }

        public async Task<EstacionamentoGetDto?> GetEstacionamentoByIdAsync(int id)
        {
            var estacionamento = await db.Estacionamentos
                .FirstOrDefaultAsync(e => e.IdEstacionamento == id)
                .ConfigureAwait(false);

            if (estacionamento == null) return null;

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
                ValorEvento = estacionamento.ValorEvento,
                HoraAbertura = estacionamento.HoraAbertura,
                HoraFechamento = estacionamento.HoraFechamento,
                Tipo = estacionamento.Tipo,
                IdGerente = estacionamento.IdGerente
            };
        }

        public async Task<EstacionamentoGetDto> AddEstacionamentoAsync(Estacionamento estacionamento)
        {
            db.Estacionamentos.Add(estacionamento);

            await db.SaveChangesAsync()
                .ConfigureAwait(false);

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
                ValorEvento = estacionamento.ValorEvento,
                HoraAbertura = estacionamento.HoraAbertura,
                HoraFechamento = estacionamento.HoraFechamento,
                Tipo = estacionamento.Tipo,
                IdGerente = estacionamento.IdGerente
            };
        }

        public async Task<bool> UpdateEstacionamentoAsync(Estacionamento estacionamento)
        {
            db.Entry(estacionamento).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync()
                    .ConfigureAwait(false);

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await db.Estacionamentos.AnyAsync(e => e.IdEstacionamento == estacionamento.IdEstacionamento).ConfigureAwait(false))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteEstacionamentoAsync(int id)
        {
            var estacionamento = await db.Estacionamentos
                .FindAsync(id)
                .ConfigureAwait(false);

            if (estacionamento == null) return false;

            db.Estacionamentos.Remove(estacionamento);

            await db.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }
    }
}
