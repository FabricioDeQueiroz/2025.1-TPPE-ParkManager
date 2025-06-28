using Microsoft.EntityFrameworkCore;
using ParkManager_Service.Data;
using ParkManager_Service.Models;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Services
{
    public class EventoService(AppDbContext db) : IEvento
    {
        public async Task<IEnumerable<EventoGetDto>> GetAllEventosAsync()
        {
            var eventos = await db.Eventos
                .Include(e => e.Estacionamento)
                .ToListAsync()
                .ConfigureAwait(false);

            var eventosDto = eventos.Select(e => new EventoGetDto
            {
                IdEvento = e.IdEvento,
                Nome = e.Nome,
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
                    ValorEvento = e.Estacionamento.ValorEvento,
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

            return new EventoGetDto
            {
                IdEvento = evento.IdEvento,
                Nome = evento.Nome,
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
                    ValorEvento = evento.Estacionamento.ValorEvento,
                    HoraAbertura = evento.Estacionamento.HoraAbertura,
                    HoraFechamento = evento.Estacionamento.HoraFechamento,
                    Tipo = evento.Estacionamento.Tipo,
                    IdGerente = evento.Estacionamento.IdGerente
                }
            };
        }

        public async Task<EventoGetDto> AddEventoAsync(EventoCreateDto evento)
        {
            var novoEvento = new Evento
            {
                Nome = evento.Nome,
                DataHoraInicio = evento.DataHoraInicio,
                DataHoraFim = evento.DataHoraFim,
                IdEstacionamento = evento.IdEstacionamento
            };

            db.Eventos.Add(novoEvento);

            await db.SaveChangesAsync()
                .ConfigureAwait(false);

            return new EventoGetDto
            {
                IdEvento = novoEvento.IdEvento,
                Nome = novoEvento.Nome,
                DataHoraInicio = novoEvento.DataHoraInicio,
                DataHoraFim = novoEvento.DataHoraFim,
                Estacionamento = new EstacionamentoGetDto
                {
                    IdEstacionamento = novoEvento.Estacionamento.IdEstacionamento,
                    Nome = novoEvento.Estacionamento.Nome,
                    NomeContratante = novoEvento.Estacionamento.NomeContratante,
                    VagasTotais = novoEvento.Estacionamento.VagasTotais,
                    VagasOcupadas = novoEvento.Estacionamento.VagasOcupadas,
                    Faturamento = novoEvento.Estacionamento.Faturamento,
                    RetornoContratante = novoEvento.Estacionamento.RetornoContratante,
                    ValorFracao = novoEvento.Estacionamento.ValorFracao,
                    DescontoHora = novoEvento.Estacionamento.DescontoHora,
                    ValorMensal = novoEvento.Estacionamento.ValorMensal,
                    ValorDiaria = novoEvento.Estacionamento.ValorDiaria,
                    AdicionalNoturno = novoEvento.Estacionamento.AdicionalNoturno,
                    ValorEvento = novoEvento.Estacionamento.ValorEvento,
                    HoraAbertura = novoEvento.Estacionamento.HoraAbertura,
                    HoraFechamento = novoEvento.Estacionamento.HoraFechamento,
                    Tipo = novoEvento.Estacionamento.Tipo,
                    IdGerente = novoEvento.Estacionamento.IdGerente
                }
            };
        }

        public async Task<bool> UpdateEventoAsync(EventoUpdateDto evento)
        {
            var eventoExistente = await db.Eventos
                .FirstOrDefaultAsync(e => e.IdEvento == evento.IdEvento)
                .ConfigureAwait(false);

            if (eventoExistente == null) return false;

            eventoExistente.Nome = evento.Nome;
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
                .FirstOrDefaultAsync(e => e.IdEvento == id)
                .ConfigureAwait(false);

            if (evento == null) return false;

            db.Eventos.Remove(evento);

            await db.SaveChangesAsync()
                .ConfigureAwait(false);

            return true;
        }
    }
}
