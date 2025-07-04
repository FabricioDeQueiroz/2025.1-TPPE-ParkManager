using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ParkManager_Service.Data;
using ParkManager_Service.Models;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Services
{
    public class EventoService(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IEvento
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        private string? GetUserId() => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private bool IsGerente() => _httpContextAccessor.HttpContext?.User.IsInRole("Gerente") ?? false;

        public async Task<IEnumerable<EventoGetDto>> GetAllEventosAsync()
        {
            string? userId = GetUserId();

            var query = db.Eventos.Include(e => e.Estacionamento).AsQueryable();

            if (IsGerente())
            {
                query = query.Where(e => e.Estacionamento.IdGerente == userId);
            }

            var eventos = await query.ToListAsync().ConfigureAwait(false);

            var eventosDto = eventos.Select(e => new EventoGetDto
            {
                IdEvento = e.IdEvento,
                Nome = e.Nome,
                ValorEvento = e.ValorEvento,
                DataHoraInicio = e.DataHoraInicio,
                DataHoraFim = e.DataHoraFim,
                Estacionamento = new EstacionamentoGetDto
                {
                    IdEstacionamento = e.Estacionamento.IdEstacionamento,
                    Nome = e.Estacionamento.Nome,
                    NomeContratante = e.Estacionamento.NomeContratante,
                    VagasTotais = e.Estacionamento.VagasTotais,
                    VagasOcupadas = e.Estacionamento.VagasOcupadas,
                    Faturamento = e.Estacionamento.Faturamento,
                    RetornoContratante = e.Estacionamento.RetornoContratante,
                    ValorFracao = e.Estacionamento.ValorFracao,
                    DescontoHora = e.Estacionamento.DescontoHora,
                    ValorMensal = e.Estacionamento.ValorMensal,
                    ValorDiaria = e.Estacionamento.ValorDiaria,
                    AdicionalNoturno = e.Estacionamento.AdicionalNoturno,
                    HoraAbertura = e.Estacionamento.HoraAbertura,
                    HoraFechamento = e.Estacionamento.HoraFechamento,
                    Tipo = e.Estacionamento.Tipo,
                    IdGerente = e.Estacionamento.IdGerente
                }
            }).ToList();

            return eventosDto;
        }

        public async Task<EventoGetDto?> GetEventoByIdAsync(int id)
        {
            var evento = await db.Eventos
                .Include(e => e.Estacionamento)
                .FirstOrDefaultAsync(e => e.IdEvento == id)
                .ConfigureAwait(false);

            if (evento == null) return null;

            if (IsGerente() && evento.Estacionamento.IdGerente != GetUserId()) return null;

            return new EventoGetDto
            {
                IdEvento = evento.IdEvento,
                Nome = evento.Nome,
                ValorEvento = evento.ValorEvento,
                DataHoraInicio = evento.DataHoraInicio,
                DataHoraFim = evento.DataHoraFim,
                Estacionamento = new EstacionamentoGetDto
                {
                    IdEstacionamento = evento.Estacionamento.IdEstacionamento,
                    Nome = evento.Estacionamento.Nome,
                    NomeContratante = evento.Estacionamento.NomeContratante,
                    VagasTotais = evento.Estacionamento.VagasTotais,
                    VagasOcupadas = evento.Estacionamento.VagasOcupadas,
                    Faturamento = evento.Estacionamento.Faturamento,
                    RetornoContratante = evento.Estacionamento.RetornoContratante,
                    ValorFracao = evento.Estacionamento.ValorFracao,
                    DescontoHora = evento.Estacionamento.DescontoHora,
                    ValorMensal = evento.Estacionamento.ValorMensal,
                    ValorDiaria = evento.Estacionamento.ValorDiaria,
                    AdicionalNoturno = evento.Estacionamento.AdicionalNoturno,
                    HoraAbertura = evento.Estacionamento.HoraAbertura,
                    HoraFechamento = evento.Estacionamento.HoraFechamento,
                    Tipo = evento.Estacionamento.Tipo,
                    IdGerente = evento.Estacionamento.IdGerente
                }
            };
        }

        // TODO verificar horário de abertura e fechamento do estacionamento
        public async Task<EventoGetDto> AddEventoAsync(EventoCreateDto evento)
        {
            var estacionamento = await db.Estacionamentos
                .FirstOrDefaultAsync(e => e.IdEstacionamento == evento.IdEstacionamento)
                .ConfigureAwait(false);

            // TODO depois arrumar essas exceções pois geram erro 500 - Internal Server Error
            if (estacionamento == null)
            {
                throw new InvalidOperationException("Estacionamento não encontrado.");
            }

            if (IsGerente() && estacionamento.IdGerente != GetUserId())
            {
                throw new UnauthorizedAccessException("Gerente não tem permissão para adicionar eventos neste estacionamento.");
            }

            var novoEvento = new Evento
            {
                Nome = evento.Nome,
                ValorEvento = evento.ValorEvento,
                DataHoraInicio = evento.DataHoraInicio,
                DataHoraFim = evento.DataHoraFim,
                IdEstacionamento = evento.IdEstacionamento
            };

            db.Eventos.Add(novoEvento);

            await db.SaveChangesAsync().ConfigureAwait(false);

            return new EventoGetDto
            {
                IdEvento = novoEvento.IdEvento,
                Nome = novoEvento.Nome,
                ValorEvento = novoEvento.ValorEvento,
                DataHoraInicio = novoEvento.DataHoraInicio,
                DataHoraFim = novoEvento.DataHoraFim,
                Estacionamento = new EstacionamentoGetDto
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
            };
        }

        public async Task<bool> UpdateEventoAsync(EventoUpdateDto evento)
        {
            var eventoExistente = await db.Eventos
                .Include(e => e.Estacionamento)
                .FirstOrDefaultAsync(e => e.IdEvento == evento.IdEvento)
                .ConfigureAwait(false);

            if (eventoExistente == null) return false;

            if (IsGerente() && eventoExistente.Estacionamento.IdGerente != GetUserId()) return false;

            eventoExistente.Nome = evento.Nome;
            eventoExistente.ValorEvento = evento.ValorEvento;
            eventoExistente.DataHoraInicio = evento.DataHoraInicio;
            eventoExistente.DataHoraFim = evento.DataHoraFim;
            eventoExistente.IdEstacionamento = evento.IdEstacionamento;

            db.Eventos.Update(eventoExistente);

            await db.SaveChangesAsync()
                .ConfigureAwait(false);

            return true;
        }

        public async Task<bool> DeleteEventoAsync(int id)
        {
            var evento = await db.Eventos
                .Include(e => e.Estacionamento)
                .FirstOrDefaultAsync(e => e.IdEvento == id)
                .ConfigureAwait(false);

            if (evento == null) return false;

            if (IsGerente() && evento.Estacionamento.IdGerente != GetUserId()) return false;

            db.Eventos.Remove(evento);

            await db.SaveChangesAsync()
                .ConfigureAwait(false);

            return true;
        }
    }
}
