import SmallCard from '../components/Dashboard/SmallCard';
import Breadcrumb from '../components/NavBar/Breadcrumb';
import { MdOutlineAttachMoney, MdExitToApp } from 'react-icons/md';
import { FaCar } from 'react-icons/fa';
import { IoNewspaperSharp } from 'react-icons/io5';
import BillingChart from '../components/Dashboard/BillingChart';
import AccessChart from '../components/Dashboard/AccessChart';
import useGetAcessos from '../Hooks/GetAcessos';
import DashboardTable from '../components/Dashboard/DashboardTable';
import DashboardOccupationTable from '../components/Dashboard/DashboardOccupationTable';
import useGetEstacionamentos from '../Hooks/GetEstacionamentos';

const DashboardManager = () => {
    const { acessos, loading, erro, superDados } = useGetAcessos();
    const { estacionamentos } = useGetEstacionamentos();

    const {
        faturamentoDoDia,
        acessosAbertosHoje,
        acessosUltimos30Dias,
        faturamentoUltimos30Dias,
        faturamentoOntem,
        acessosOntem,
        faturamentoMesPassado,
        acessosMesPassado,
        acessosAgrupados,
    } = superDados;

    if (loading) {
        return (
            <div className="flex flex-col h-[calc(100vh-52px)] max-w-[calc(100vh-256px)] min-w-fit mx-5 mb-5 mt-8">
                <div className="mb-2">
                    <Breadcrumb
                        lista={['ParkManager', 'Painel']}
                        caminhos={['', '/dashboard']}
                    />
                </div>

                <div className="text-xl font-bold mb-2 text-text-page-title">
                    Painel
                </div>

                <div className="flex flex-row justify-center items-center w-[1580px]">
                    <span className="loading loading-ring w-3xl text-dashboard-green-400"></span>
                </div>
            </div>
        );
    }

    if (erro) {
        return (
            <div className="flex flex-col h-[calc(100vh-52px)] max-w-[calc(100vh-256px)] min-w-fit mx-5 mb-5 mt-8">
                <div className="mb-2">
                    <Breadcrumb
                        lista={['ParkManager', 'Painel']}
                        caminhos={['', '/dashboard']}
                    />
                </div>

                <div className="text-xl font-bold mb-2 text-text-page-title">
                    Painel
                </div>

                <div className="w-[1580px] mt-8">
                    <span className="text-dashboard-red-400 text-2xl font-bold">
                        {erro}
                    </span>
                </div>
            </div>
        );
    }

    return (
        <div className="flex flex-col h-[calc(100vh)] max-w-[calc(100vh-256px)] min-w-fit mx-5 mb-5 mt-8">
            <div className="mb-2">
                <Breadcrumb
                    lista={['ParkManager', 'Painel']}
                    caminhos={['', '/dashboard']}
                />
            </div>

            <div className="text-xl font-bold mb-2 text-text-page-title">
                Painel
            </div>

            <div className="flex flex-row justify-between gap-x-7 mb-5">
                <SmallCard
                    titulo={'FATURAMENTO DIÁRIO'}
                    corpo={`R$ ${faturamentoDoDia.toFixed(2).replace('.', ',')}`}
                    plus={
                        faturamentoOntem > 0
                            ? faturamentoDoDia >= faturamentoOntem
                                ? true
                                : false
                            : faturamentoDoDia > 0
                              ? true
                              : false
                    }
                    quantidade={
                        faturamentoOntem > 0
                            ? `${((faturamentoDoDia / faturamentoOntem) * 100 - 100).toFixed(2)}%`
                            : '(sem faturamento ontem)'
                    }
                    periodo={faturamentoOntem > 0 ? 'que ontem' : ''}
                    icon={MdOutlineAttachMoney}
                    signal={
                        faturamentoOntem > 0
                            ? faturamentoDoDia >= faturamentoOntem
                                ? '+'
                                : ''
                            : ''
                    }
                />
                <SmallCard
                    titulo={'ACESSOS DIÁRIOS'}
                    corpo={acessosAbertosHoje}
                    plus={
                        acessosOntem > 0
                            ? acessosAbertosHoje >= acessosOntem
                                ? true
                                : false
                            : acessosAbertosHoje > 0
                              ? true
                              : false
                    }
                    quantidade={
                        acessosOntem > 0
                            ? `${((acessosAbertosHoje / acessosOntem) * 100 - 100).toFixed(2)}%`
                            : '(sem acessos ontem)'
                    }
                    periodo={acessosOntem > 0 ? 'que ontem' : ''}
                    icon={FaCar}
                    signal={
                        acessosOntem > 0
                            ? acessosAbertosHoje >= acessosOntem
                                ? '+'
                                : ''
                            : ''
                    }
                />
                <SmallCard
                    titulo={'ACESSOS MENSAIS'}
                    corpo={acessosUltimos30Dias}
                    plus={
                        acessosMesPassado > 0
                            ? acessosUltimos30Dias >= acessosMesPassado
                                ? true
                                : false
                            : acessosUltimos30Dias > 0
                              ? true
                              : false
                    }
                    quantidade={
                        acessosMesPassado > 0
                            ? `${((acessosUltimos30Dias / acessosMesPassado) * 100 - 100).toFixed(2)}%`
                            : '(sem acessos mês passado)'
                    }
                    periodo={acessosMesPassado > 0 ? 'que mês passado' : ''}
                    icon={MdExitToApp}
                    signal={
                        acessosMesPassado > 0
                            ? acessosUltimos30Dias >= acessosMesPassado
                                ? '+'
                                : ''
                            : ''
                    }
                />
                <SmallCard
                    titulo={'FATURAMENTO MENSAL'}
                    corpo={`R$ ${faturamentoUltimos30Dias.toFixed(2).replace('.', ',')}`}
                    plus={
                        faturamentoMesPassado > 0
                            ? faturamentoUltimos30Dias >= faturamentoMesPassado
                                ? true
                                : false
                            : faturamentoUltimos30Dias > 0
                              ? true
                              : false
                    }
                    quantidade={
                        faturamentoMesPassado > 0
                            ? `${((faturamentoUltimos30Dias / faturamentoMesPassado) * 100 - 100).toFixed(2)}%`
                            : '(sem faturamento mês passado)'
                    }
                    periodo={faturamentoMesPassado > 0 ? 'que mês passado' : ''}
                    icon={IoNewspaperSharp}
                    signal={
                        faturamentoMesPassado > 0
                            ? faturamentoUltimos30Dias >= faturamentoMesPassado
                                ? '+'
                                : ''
                            : ''
                    }
                />
            </div>

            <div className="flex flex-row justify-between gap-x-5 mb-5">
                <BillingChart
                    title="FATURAMENTO NO DECORRER DOS MESES"
                    data={acessosAgrupados}
                />
                <AccessChart
                    title="ACESSOS NO DECORRER DOS MESES"
                    data={acessosAgrupados}
                />
            </div>

            <div className="flex flex-row justify-between gap-x-5">
                <DashboardTable data={acessos} limit={5} />

                <DashboardOccupationTable data={estacionamentos} limit={3} />
            </div>
        </div>
    );
};

export default DashboardManager;
