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
            string? userId = GetUserId();

            var query = db.Acessos
                .Include(c => c.Cliente)
                .Include(e => e.Estacionamento)
                .Include(ev => ev.Evento)
                .AsQueryable();

            if (IsCliente())
            {
                query = query.Where(a => a.IdCliente == userId);
            }

            if (IsGerente())
            {
                query = query.Where(a => a.Estacionamento.IdGerente == userId);
            }

            var acessos = await query.ToListAsync().ConfigureAwait(false);

            var resultados = await Task.WhenAll(
                acessos.Select(e => IdentifyTypeAndValueOfAccessoAsync(e.IdAcesso, false))
            ).ConfigureAwait(false);

            var acessosDto = resultados
                .Where(r => r is { Success: true, Data: not null })
                .Select(r => r.Data!)
                .ToList();

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

            var resultado = await IdentifyTypeAndValueOfAccessoAsync(acesso.IdAcesso, false).ConfigureAwait(false);

            if (resultado is { Success: true, Data: not null })
            {
                return resultado.Data;
            }

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
                Evento = acesso.Evento == null ? null : new EventoForAcessoGetDto
                {
                    IdEvento = acesso.Evento.IdEvento,
                    Nome = acesso.Evento.Nome,
                    ValorEvento = acesso.Evento.ValorEvento,
                    DataHoraInicio = acesso.Evento.DataHoraInicio,
                    DataHoraFim = acesso.Evento.DataHoraFim
                }
            };
        }

        public async Task<Resultado<AcessoGetDto>> AddAcessoAsync(AcessoCreateDto acesso)
        {
            string? userId = GetUserId();

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

            // Verifica se o estacionamento está aberto no horário atual (se não for 24 horas):
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
                    DataHoraSaida = null,
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

            var estacionamento = await db.Estacionamentos
                .FirstOrDefaultAsync(e => e.IdEstacionamento == acessoExistente.IdEstacionamento)
                .ConfigureAwait(false);

            if (estacionamento == null) return false;

            db.Acessos.Remove(acessoExistente);

            if (estacionamento.VagasOcupadas > 0) estacionamento.VagasOcupadas--;

            db.Estacionamentos.Update(estacionamento);

            await db.SaveChangesAsync()
                .ConfigureAwait(false);

            return true;
        }

        public async Task<Resultado<AcessoGetDto>> IdentifyTypeAndValueOfAccessoAsync(int id, bool encerrar)
        {
            string? userId = GetUserId();

            if (userId == null)
            {
                return Resultado<AcessoGetDto>.Falha("Falha ao obter ID do Usuário.");
            }

            // Verifica se o acesso existe:
            var acesso = await db.Acessos
                .FirstOrDefaultAsync(a => a.IdAcesso == id)
                .ConfigureAwait(false);

            if (acesso == null)
            {
                return Resultado<AcessoGetDto>.Falha("Acesso não encontrado.");
            }

            // Verifica se o usuário autenticado é o cliente do acesso:
            if (IsCliente() && acesso.IdCliente != userId)
            {
                return Resultado<AcessoGetDto>.Falha("Acesso não pertence ao cliente autenticado.");
            }

            // Verifica se o Acesso já foi finalizado:
            if (acesso.DataHoraSaida != null)
            {
                return Resultado<AcessoGetDto>.Falha("Acesso já foi finalizado.");
            }

            // Verifica se o estacionamento existe:
            var estacionamento = await db.Estacionamentos
                .FirstOrDefaultAsync(e => e.IdEstacionamento == acesso.IdEstacionamento)
                .ConfigureAwait(false);

            if (estacionamento == null)
            {
                return Resultado<AcessoGetDto>.Falha("Estacionamento não encontrado.");
            }

            // Verifica se o evento existe:
            var evento = await db.Eventos
                .FirstOrDefaultAsync(e => e.IdEvento == acesso.IdEvento)
                .ConfigureAwait(false);

            if (evento == null)
            {
                return Resultado<AcessoGetDto>.Falha("Evento não encontrado.");
            }

            // Verifica se o Gerente do estacionamento é o usuário autenticado:
            if (IsGerente() && estacionamento.IdGerente != userId)
            {
                return Resultado<AcessoGetDto>.Falha("Acesso não pertence ao estacionamento do gerente autenticado.");
            }

            // Pega o cliente associado ao usuário autenticado:
            var cliente = await db.Usuarios
                .FirstOrDefaultAsync(u => u.Id == userId)
                .ConfigureAwait(false);

            if (cliente == null)
            {
                return Resultado<AcessoGetDto>.Falha("Cliente não encontrado.");
            }

            // Verificar Tipo e Valor do Acesso:
            var dataHoraAtual = DateTime.Now;

            var dataHoraSaida = acesso.DataHoraSaida ?? dataHoraAtual;
            var dataHoraEntrada = acesso.DataHoraEntrada;

            if (encerrar && estacionamento.Tipo == TipoEstacionamento.Comum)
            {
                if (dataHoraAtual.TimeOfDay > estacionamento.HoraFechamento || dataHoraAtual.TimeOfDay < estacionamento.HoraAbertura)
                {
                    return Resultado<AcessoGetDto>.Falha("Aguarde a abertura do Estacionamento para finalizar seu acesso.");
                }
            }

            if (acesso.Tipo == TipoAcesso.Evento)
            {
                if (dataHoraAtual <= evento.DataHoraFim)
                    return Resultado<AcessoGetDto>.Ok(
                        new AcessoGetDto
                        {
                            IdAcesso = acesso.IdAcesso,
                            PlacaVeiculo = acesso.PlacaVeiculo,
                            ValorAcesso = acesso.ValorAcesso,
                            DataHoraEntrada = acesso.DataHoraEntrada,
                            DataHoraSaida = acesso.DataHoraSaida,
                            Tipo = acesso.Tipo,
                            Cliente = new UsuarioGetDto
                            {
                                Id = cliente.Id,
                                Nome = cliente.Nome,
                                Email = cliente.Email!,
                                Tipo = cliente.Tipo
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

                var duracaoAdicional = evento.DataHoraFim - dataHoraAtual;

                double horasAdicionais = duracaoAdicional.TotalHours;
                double minutosAdicionais = duracaoAdicional.TotalMinutes;

                int blocos15Adicionais = (int)Math.Ceiling(minutosAdicionais / 15);
                int horasCheiasAdicionais = (int)horasAdicionais;
                decimal valorTempoAdicionais = blocos15Adicionais * estacionamento.ValorFracao - horasCheiasAdicionais * estacionamento.DescontoHora;
                valorTempoAdicionais = Math.Max(valorTempoAdicionais, 0);

                acesso.ValorAcesso = evento.ValorEvento + valorTempoAdicionais;

                if (encerrar)
                {
                    acesso.DataHoraSaida = dataHoraSaida;

                    if (estacionamento.VagasOcupadas > 0) estacionamento.VagasOcupadas--;
                    estacionamento.Faturamento += acesso.ValorAcesso ?? evento.ValorEvento + valorTempoAdicionais;
                }

                db.Acessos.Update(acesso);
                db.Estacionamentos.Update(estacionamento);

                await db.SaveChangesAsync().ConfigureAwait(false);

                return Resultado<AcessoGetDto>.Ok(
                    new AcessoGetDto
                    {
                        IdAcesso = acesso.IdAcesso,
                        PlacaVeiculo = acesso.PlacaVeiculo,
                        ValorAcesso = acesso.ValorAcesso,
                        DataHoraEntrada = acesso.DataHoraEntrada,
                        DataHoraSaida = acesso.DataHoraSaida,
                        Tipo = acesso.Tipo,
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

            var duracao = dataHoraSaida - dataHoraEntrada;

            double dias = duracao.TotalDays;
            double horas = duracao.TotalHours;
            double minutos = duracao.TotalMinutes;

            // Tolerância: até 14min59s = PorTempo com valor zero
            if (minutos < 15)
            {
                acesso.Tipo = TipoAcesso.PorTempo;
                acesso.ValorAcesso = 0;

                if (encerrar)
                {
                    acesso.DataHoraSaida = dataHoraAtual;

                    if (estacionamento.VagasOcupadas > 0) estacionamento.VagasOcupadas--;
                }

                db.Acessos.Update(acesso);
                db.Estacionamentos.Update(estacionamento);

                await db.SaveChangesAsync().ConfigureAwait(false);

                return Resultado<AcessoGetDto>.Ok(
                    new AcessoGetDto
                    {
                        IdAcesso = acesso.IdAcesso,
                        PlacaVeiculo = acesso.PlacaVeiculo,
                        ValorAcesso = acesso.ValorAcesso,
                        DataHoraEntrada = acesso.DataHoraEntrada,
                        DataHoraSaida = acesso.DataHoraSaida,
                        Tipo = acesso.Tipo,
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

            // Calcular valor mensal
            int meses = (int)(dias / 30);
            double restoDiasMensal = dias % 30;
            decimal valorMensal = meses * estacionamento.ValorMensal;

            // Calcular valor diária e adicional noturno do restante
            int diasInteirosRestantes = (int)restoDiasMensal;
            double horasRestantes = (restoDiasMensal - diasInteirosRestantes) * 24;

            decimal valorDiariaRestante = diasInteirosRestantes * estacionamento.ValorDiaria;

            if (dataHoraSaida.Hour < 6 || dataHoraEntrada.Hour >= 22)
                valorDiariaRestante += estacionamento.AdicionalNoturno;

            // Calcular valor por tempo do restante
            int blocos15Restante = (int)Math.Ceiling(horasRestantes * 60 / 15);
            int horasCheiasRestante = (int)horasRestantes;
            decimal valorTempoRestante = blocos15Restante * estacionamento.ValorFracao - horasCheiasRestante * estacionamento.DescontoHora;
            valorTempoRestante = Math.Max(valorTempoRestante, 0);

            decimal valorTotalMensal = valorMensal + valorDiariaRestante + valorTempoRestante;

            // Calcular valor total apenas por diária
            decimal valorTotalDiaria = (int)dias * estacionamento.ValorDiaria;
            if (dataHoraSaida.Hour < 6 || dataHoraEntrada.Hour >= 22)
                valorTotalDiaria += estacionamento.AdicionalNoturno;

            // Calcular valor total apenas por tempo
            int blocos15 = (int)Math.Ceiling(minutos / 15);
            int horasCheias = (int)horas;
            decimal valorTotalPorTempo = blocos15 * estacionamento.ValorFracao - horasCheias * estacionamento.DescontoHora;
            valorTotalPorTempo = Math.Max(valorTotalPorTempo, 0);

            // Avaliar melhor opção (mínimo valor)
            var opcoes = new List<(TipoAcesso tipo, decimal valor)>
            {
                (TipoAcesso.PorTempo, valorTotalPorTempo),
                (TipoAcesso.Diaria, valorTotalDiaria),
                (TipoAcesso.Mensal, valorTotalMensal)
            };

            var melhor = opcoes.OrderBy(x => x.valor).First();
            acesso.Tipo = melhor.tipo;
            acesso.ValorAcesso = melhor.valor;

            if (encerrar)
            {
                acesso.DataHoraSaida = dataHoraSaida;

                if (estacionamento.VagasOcupadas > 0) estacionamento.VagasOcupadas--;
                estacionamento.Faturamento += melhor.valor;
            }

            db.Acessos.Update(acesso);
            db.Estacionamentos.Update(estacionamento);

            await db.SaveChangesAsync().ConfigureAwait(false);

            return Resultado<AcessoGetDto>.Ok(
                new AcessoGetDto
                {
                    IdAcesso = acesso.IdAcesso,
                    PlacaVeiculo = acesso.PlacaVeiculo,
                    ValorAcesso = acesso.ValorAcesso,
                    DataHoraEntrada = acesso.DataHoraEntrada,
                    DataHoraSaida = acesso.DataHoraSaida,
                    Tipo = acesso.Tipo,
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
    }
}
