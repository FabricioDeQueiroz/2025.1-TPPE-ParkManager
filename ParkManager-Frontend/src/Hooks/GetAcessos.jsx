import { useEffect, useState } from 'react';
import { API_BASE_URL } from '../util/Constants';
import { useAuth } from '../features/auth/AuthContext';
import { subDays, isSameDay, parseISO, isWithinInterval } from 'date-fns';
import axios from 'axios';

const useGetAcessos = () => {
    const [acessos, setAcessos] = useState([]);
    const [loading, setLoading] = useState(true);
    const [erro, setErro] = useState(null);

    const { token } = useAuth();

    useEffect(() => {
        const fetchAcessos = async () => {
            try {
                const response = await axios.get(`${API_BASE_URL}/Acesso`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
                setAcessos(response.data);
            } catch (err) {
                setErro(err);
            } finally {
                setLoading(false);
            }
        };

        fetchAcessos();
    }, [token]);

    const hoje = new Date();
    const ontem = subDays(hoje, 1);
    const trintaDiasAtras = subDays(hoje, 29);
    const sessentaDiasAtras = subDays(hoje, 59);

    const isHoje = (dataStr) => isSameDay(parseISO(dataStr), hoje);
    const isUltimos30Dias = (dataStr) =>
        isWithinInterval(parseISO(dataStr), {
            start: trintaDiasAtras,
            end: hoje,
        });
    const isOntem = (dataStr) => isSameDay(parseISO(dataStr), ontem);
    const isMesPassado = (dataStr) =>
        isWithinInterval(parseISO(dataStr), {
            start: sessentaDiasAtras,
            end: trintaDiasAtras,
        });

    // 1. Faturamento do dia (apenas acessos que saíram hoje)
    const faturamentoDoDia = acessos
        .filter((a) => a.dataHoraSaida && isHoje(a.dataHoraSaida))
        .reduce((total, a) => total + (a.valorAcesso || 0), 0);

    // 2. Acessos abertos no dia atual
    const acessosAbertosHoje = acessos.filter((a) =>
        isHoje(a.dataHoraEntrada)
    ).length;

    // 3. Acessos no mês atual (últimos 30 dias)
    const acessosUltimos30Dias = acessos.filter((a) =>
        isUltimos30Dias(a.dataHoraEntrada)
    ).length;

    // 4. Faturamento do mês (últimos 30 dias com dataHoraSaida preenchida)
    const faturamentoUltimos30Dias = acessos
        .filter((a) => a.dataHoraSaida && isUltimos30Dias(a.dataHoraSaida))
        .reduce((total, a) => total + (a.valorAcesso || 0), 0);

    // 5. Faturamento de ontem
    const faturamentoOntem = acessos
        .filter((a) => a.dataHoraSaida && isOntem(a.dataHoraSaida))
        .reduce((total, a) => total + (a.valorAcesso || 0), 0);

    // 6. Acessos abertos ontem
    const acessosOntem = acessos.filter((a) =>
        isOntem(a.dataHoraEntrada)
    ).length;

    // 7. Faturamento do mês passado (30–60 dias atrás)
    const faturamentoMesPassado = acessos
        .filter((a) => a.dataHoraSaida && isMesPassado(a.dataHoraSaida))
        .reduce((total, a) => total + (a.valorAcesso || 0), 0);

    // 8. Acessos do mês passado (30–60 dias atrás)
    const acessosMesPassado = acessos.filter((a) =>
        isMesPassado(a.dataHoraEntrada)
    ).length;

    // 9. Primeiro acesso
    const primeiroAcesso = acessos[0];

    const meses = [
        'Jan',
        'Fev',
        'Mar',
        'Abr',
        'Mai',
        'Jun',
        'Jul',
        'Ago',
        'Set',
        'Out',
        'Nov',
        'Dez',
    ];

    function agruparAcessosPorMes(acessos) {
        const resultado = Array.from({ length: 12 }, (_, i) => ({
            name: meses[i],
            valorTotal: 0,
            quantidade: 0,
        }));

        acessos.forEach((acesso) => {
            if (acesso.dataHoraEntrada) {
                const entrada = parseISO(acesso.dataHoraEntrada);
                const mesEntrada = entrada.getMonth();
                resultado[mesEntrada].quantidade += 1;
            }

            if (acesso.dataHoraSaida) {
                const saida = parseISO(acesso.dataHoraSaida);
                const mesSaida = saida.getMonth();
                resultado[mesSaida].valorTotal += acesso.valorAcesso || 0;
            }
        });

        return resultado;
    }

    const acessosAgrupados = agruparAcessosPorMes(acessos);

    const superDados = {
        faturamentoDoDia,
        acessosAbertosHoje,
        acessosUltimos30Dias,
        faturamentoUltimos30Dias,
        faturamentoOntem,
        acessosOntem,
        faturamentoMesPassado,
        acessosMesPassado,
        acessosAgrupados,
        primeiroAcesso,
    };

    return { acessos, loading, erro, superDados };
};

export default useGetAcessos;
