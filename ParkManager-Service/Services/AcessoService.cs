using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ParkManager_Service.Data;
using ParkManager_Service.Helpers;
using ParkManager_Service.Models;
using ParkManager_Service.Services.Interfaces;
using ParkManager_Service.Views;

namespace ParkManager_Service.Services
{
    public class AcessoService(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IAcesso
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        private string? GetUserId() => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private bool IsCliente() => _httpContextAccessor.HttpContext?.User.IsInRole("Cliente") ?? false;
        
        private bool IsGerente() => _httpContextAccessor.HttpContext?.User.IsInRole("Gerente") ?? false;

        public async Task<IEnumerable<AcessoGetDto>> GetAllAcessosAsync()
        {
            var query = db.Acessos
                .Include(c => c.Cliente)
                .Include(e => e.Estacionamento)
                .Include(ev => ev.Evento)
                .AsQueryable();

            if (IsCliente())
            {
                query = query.Where(a => a.IdCliente == GetUserId());
            }
            
            if (IsGerente())
            {
                query = query.Where(a => a.Estacionamento.IdGerente == GetUserId());
            }

            var acessos = await query.ToListAsync().ConfigureAwait(false);

            var acessosDto = acessos.Select(e => new AcessoGetDto
            {
                IdAcesso = e.IdAcesso,
                PlacaVeiculo = e.PlacaVeiculo,
                ValorAcesso = e.ValorAcesso,
                DataHoraEntrada = e.DataHoraEntrada,
                DataHoraSaida = e.DataHoraSaida,
                Tipo = e.Tipo,
                Cliente = new UsuarioGetDto
                {
                    Id = e.Cliente.Id,
                    Nome = e.Cliente.Nome,
                    Email = e.Cliente.Email!,
                    Tipo = e.Cliente.Tipo,
                },
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
                },
                Evento = new EventoForAcessoGetDto
                {
                    IdEvento = e.Evento?.IdEvento ?? -1,
                    Nome = e.Evento?.Nome ?? string.Empty,
                    ValorEvento = e.Evento?.ValorEvento ?? -1,
                    DataHoraInicio = e.Evento?.DataHoraInicio ?? DateTime.MinValue,
                    DataHoraFim = e.Evento?.DataHoraFim ?? DateTime.MinValue
                }
            }).ToList();

            return acessosDto;
        }

        public async Task<AcessoGetDto?> GetAcessoByIdAsync(int id)
        {
            var acesso = await db.Acessos
                .Include(c => c.Cliente)
                .Include(e => e.Estacionamento)
                .Include(ev => ev.Evento)
                .FirstOrDefaultAsync(a => a.IdAcesso == id)
                .ConfigureAwait(false);

            if (acesso == null) return null;

            if (IsCliente() && acesso.IdCliente != GetUserId()) return null;

            if (IsGerente() && acesso.Estacionamento.IdGerente != GetUserId()) return null;

            return new AcessoGetDto
            {
                IdAcesso = acesso.IdAcesso,
                PlacaVeiculo = acesso.PlacaVeiculo,
                ValorAcesso = acesso.ValorAcesso,
                DataHoraEntrada = acesso.DataHoraEntrada,
                DataHoraSaida = acesso.DataHoraSaida,
                Tipo = acesso.Tipo,
                Cliente = new UsuarioGetDto
                {
                    Id = acesso.Cliente.Id,
                    Nome = acesso.Cliente.Nome,
                    Email = acesso.Cliente.Email!,
                    Tipo = acesso.Cliente.Tipo,
                },
                Estacionamento = new EstacionamentoGetDto
                {
                    IdEstacionamento = acesso.Estacionamento.IdEstacionamento,
                    Nome = acesso.Estacionamento.Nome,
                    NomeContratante = acesso.Estacionamento.NomeContratante,
                    VagasTotais = acesso.Estacionamento.VagasTotais,
                    VagasOcupadas = acesso.Estacionamento.VagasOcupadas,
                    Faturamento = acesso.Estacionamento.Faturamento,
                    RetornoContratante = acesso.Estacionamento.RetornoContratante,
                    ValorFracao = acesso.Estacionamento.ValorFracao,
                    DescontoHora = acesso.Estacionamento.DescontoHora,
                    ValorMensal = acesso.Estacionamento.ValorMensal,
                    ValorDiaria = acesso.Estacionamento.ValorDiaria,
                    AdicionalNoturno = acesso.Estacionamento.AdicionalNoturno,
                    HoraAbertura = acesso.Estacionamento.HoraAbertura,
                    HoraFechamento = acesso.Estacionamento.HoraFechamento,
                    Tipo = acesso.Estacionamento.Tipo,
                    IdGerente = acesso.Estacionamento.IdGerente
                },
                Evento = new EventoForAcessoGetDto
                {
                    IdEvento = acesso.Evento?.IdEvento ?? -1,
                    Nome = acesso.Evento?.Nome ?? string.Empty,
                    ValorEvento = acesso.Evento?.ValorEvento ?? -1,
                    DataHoraInicio = acesso.Evento?.DataHoraInicio ?? DateTime.MinValue,
                    DataHoraFim = acesso.Evento?.DataHoraFim ?? DateTime.MinValue
                }
            };
        }

        public async Task<Resultado<AcessoGetDto>> AddAcessoAsync(AcessoCreateDto acesso)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Resultado<AcessoGetDto>.Falha("Falha ao obter ID do Usuário.");
            }

            // Verifica se o estacionamento existe:
            var estacionamento = await db.Estacionamentos
                .FirstOrDefaultAsync(e => e.IdEstacionamento == acesso.IdEstacionamento)
                .ConfigureAwait(false);

            if (estacionamento == null)
            {
                return Resultado<AcessoGetDto>.Falha("Estacionamento não encontrado.");
            }

            // Verifica se o evento existe (caso tenha sido informado):
            var evento = await db.Eventos
                .FirstOrDefaultAsync(e => e.IdEvento == acesso.IdEvento)
                .ConfigureAwait(false);
                
            if (evento == null)
            {
                return Resultado<AcessoGetDto>.Falha("Evento não encontrado.");
            }

            // Pega o cliente associado ao usuário autenticado:
            var cliente = await db.Usuarios
                .FirstOrDefaultAsync(u => u.Id == userId)
                .ConfigureAwait(false);

            if (cliente == null)
            {
                return Resultado<AcessoGetDto>.Falha("Cliente não encontrado.");
            }

            // Verifica se o evento pertence ao estacionamento:
            if (evento.IdEstacionamento != estacionamento.IdEstacionamento)
            {
                return Resultado<AcessoGetDto>.Falha("Evento não pertence ao estacionamento especificado.");
            }

            var dataHoraAtual = DateTime.Now;

            // Verifica se o estacionamento está aberto no horário atual (se não for 24h):
            if (estacionamento.Tipo == TipoEstacionamento.Comum)
            {
                if (dataHoraAtual.TimeOfDay < estacionamento.HoraAbertura || dataHoraAtual.TimeOfDay > estacionamento.HoraFechamento)
                {
                    return Resultado<AcessoGetDto>.Falha("O estacionamento está fechado no momento.");
                }
            }

            // Verifica se há vagas disponíveis:
            if (estacionamento.VagasOcupadas >= estacionamento.VagasTotais)
            {
                return Resultado<AcessoGetDto>.Falha("Não há vagas disponíveis no estacionamento.");
            }

            // Se o acesso não for para um evento, cria um novo acesso PorTempo:
            if (acesso.IdEvento == null)
            {

                var novoAcesso = new Acesso
                {
                    PlacaVeiculo = acesso.PlacaVeiculo,
                    ValorAcesso = 0,
                    DataHoraEntrada = dataHoraAtual,
                    DataHoraSaida = null,
                    Tipo = TipoAcesso.PorTempo,
                    IdCliente = userId,
                    IdEstacionamento = estacionamento.IdEstacionamento
                };

                db.Acessos.Add(novoAcesso);

                estacionamento.VagasOcupadas++;

                db.Estacionamentos.Update(estacionamento);

                await db.SaveChangesAsync().ConfigureAwait(false);

                return Resultado<AcessoGetDto>.Ok(
                    new AcessoGetDto
                    {
                        IdAcesso = novoAcesso.IdAcesso,
                        PlacaVeiculo = novoAcesso.PlacaVeiculo,
                        ValorAcesso = novoAcesso.ValorAcesso,
                        DataHoraEntrada = novoAcesso.DataHoraEntrada,
                        DataHoraSaida = novoAcesso.DataHoraSaida,
                        Tipo = novoAcesso.Tipo,
                        Cliente = new UsuarioGetDto
                        {
                            Id = cliente.Id,
                            Nome = cliente.Nome,
                            Email = cliente.Email!,
                            Tipo = cliente.Tipo,
                        },
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
                    }
                );
            }
            else
            {
                // Verifica se o evento está ativo no momento:
                if (dataHoraAtual < evento.DataHoraInicio || dataHoraAtual > evento.DataHoraFim)
                {
                    return Resultado<AcessoGetDto>.Falha("O evento não está ativo no momento.");
                }

                var novoAcesso = new Acesso
                {
                    PlacaVeiculo = acesso.PlacaVeiculo,
                    ValorAcesso = evento.ValorEvento,
                    DataHoraEntrada = dataHoraAtual,
                    DataHoraSaida = evento.DataHoraFim,
                    Tipo = TipoAcesso.Evento,
                    IdCliente = userId,
                    IdEstacionamento = estacionamento.IdEstacionamento,
                    IdEvento = evento.IdEvento
                };

                db.Acessos.Add(novoAcesso);

                estacionamento.VagasOcupadas++;

                db.Estacionamentos.Update(estacionamento);

                await db.SaveChangesAsync().ConfigureAwait(false);

                return Resultado<AcessoGetDto>.Ok(
                    new AcessoGetDto
                    {
                        IdAcesso = novoAcesso.IdAcesso,
                        PlacaVeiculo = novoAcesso.PlacaVeiculo,
                        ValorAcesso = novoAcesso.ValorAcesso,
                        DataHoraEntrada = novoAcesso.DataHoraEntrada,
                        DataHoraSaida = novoAcesso.DataHoraSaida,
                        Tipo = novoAcesso.Tipo,
                        Cliente = new UsuarioGetDto
                        {
                            Id = cliente.Id,
                            Nome = cliente.Nome,
                            Email = cliente.Email!,
                            Tipo = cliente.Tipo,
                        },
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
                        },
                        Evento = new EventoForAcessoGetDto
                        {
                            IdEvento = evento.IdEvento,
                            Nome = evento.Nome,
                            ValorEvento = evento.ValorEvento,
                            DataHoraInicio = evento.DataHoraInicio,
                            DataHoraFim = evento.DataHoraFim
                        }
                    }
                );
            }
        }

        public async Task<bool> UpdateAcessoAsync(AcessoUpdateDto acesso)
        {
            var acessoExistente = await db.Acessos
                .FirstOrDefaultAsync(e => e.IdAcesso == acesso.IdAcesso)
                .ConfigureAwait(false);

            if (acessoExistente == null) return false;

            if (IsCliente() && acessoExistente.IdCliente != GetUserId()) return false;

            acessoExistente.PlacaVeiculo = acesso.PlacaVeiculo;

            db.Acessos.Update(acessoExistente);

            await db.SaveChangesAsync()
                .ConfigureAwait(false);

            return true;
        }

        public async Task<bool> DeleteAcessoAsync(int id)
        {
            var acessoExistente = await db.Acessos
                .FirstOrDefaultAsync(e => e.IdAcesso == id)
                .ConfigureAwait(false);

            if (acessoExistente == null) return false;

            if (IsCliente() && acessoExistente.IdCliente != GetUserId()) return false;

            db.Acessos.Remove(acessoExistente);

            await db.SaveChangesAsync()
                .ConfigureAwait(false);

            return true;
        }

        public async Task<string> IdentifyTypeAndValueOfAccessoAsync()
        {
            return "Tipo: Por Tempo, Valor: 10.00";
        }

        public async Task<string> FinalizeAcessoAsync(int id)
        {
            return $"Acesso finalizado com sucesso.";
        }
    }
}
