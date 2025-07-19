import Breadcrumb from '../components/NavBar/Breadcrumb';
import { MdOutlineAttachMoney } from 'react-icons/md';
import { IoExitOutline } from 'react-icons/io5';
import BillingChart from '../components/Dashboard/BillingChart';
import AccessChart from '../components/Dashboard/AccessChart';
import getAcessos from '../Hooks/GetAcessos';
import getEstacionamentos from '../Hooks/GetEstacionamentos';
import SmallCardCustomer from '../components/Dashboard/SmallCardCustomer';
import AccessCard from '../components/Dashboard/AccessCard';
import { DateConverter } from '../util/DateConverter';
import DashboardTableCustomer from '../components/Dashboard/DashboardTableCustomer';

const DashboardCustomer = () => {
    const { acessos, loading, erro, superDados } = getAcessos();

    const { acessosAbertosHoje, acessosAgrupados, primeiroAcesso } = superDados;

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

            <div className="flex flex-row justify-between gap-x-7 mb-5">
                <AccessCard
                    corpo={acessosAbertosHoje}
                    entrada={DateConverter(
                        primeiroAcesso.dataHoraEntrada,
                        true,
                        0
                    )}
                />
                <SmallCardCustomer
                    titulo={'VALOR DO ACESSO ATUAL'}
                    corpo={`R$ ${primeiroAcesso.valorAcesso.toFixed(2).replace('.', ',')}`}
                    icon={MdOutlineAttachMoney}
                    width="w-[385px]"
                    fontSize="text-[34px]"
                />
                <SmallCardCustomer
                    titulo={'ENCERRAR O ACESSO ATUAL'}
                    corpo={
                        primeiroAcesso
                            ? `${primeiroAcesso.placaVeiculo} no ${primeiroAcesso.estacionamento.nome}`
                            : 'N/A'
                    }
                    icon={IoExitOutline}
                    width="w-[511px]"
                    widthCorpo="w-[380px]"
                    button={true}
                    to="/acessos/"
                    fontSize="text-lg"
                />
            </div>

            <div className="flex flex-row justify-between gap-x-7 mb-5">
                <BillingChart
                    title="GASTO NO DECORRER DOS MESES"
                    data={acessosAgrupados}
                />
                <AccessChart
                    title="ACESSOS PESSOAIS NO DECORRER DOS MESES"
                    data={acessosAgrupados}
                />
            </div>

            <div className="flex flex-row justify-between gap-x-5">
                <DashboardTableCustomer data={acessos} limit={5} />
            </div>
        </div>
    );
};

export default DashboardCustomer;
