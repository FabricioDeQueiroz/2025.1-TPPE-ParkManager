import SmallCard from '../components/Dashboard/SmallCard';
import Breadcrumb from '../components/NavBar/Breadcrumb';
import { MdOutlineAttachMoney, MdExitToApp } from 'react-icons/md';
import { FaCar } from 'react-icons/fa';
import { IoNewspaperSharp } from 'react-icons/io5';
import BillingChart from '../components/Dashboard/BillingChart';
import AccessChart from '../components/Dashboard/AccessChart';

const DashboardManager = () => {
    return (
        <div className="flex flex-col h-[calc(100vh-52px)] max-w-[calc(100vh-256px)] min-w-fit mx-5 mb-5 mt-8">
            <div className="mb-2">
                <Breadcrumb
                    lista={['ParkManager', 'Painel']}
                    caminhos={['', '/dashboard']}
                />
            </div>

            <div className="text-xl font-bold mb-4 text-text-page-title">
                Painel
            </div>

            <div className="flex flex-row justify-between gap-x-7 mb-5">
                {/* TODO colocar valores dinâmicos */}
                <SmallCard
                    titulo={'FATURAMENTO DIÁRIO'}
                    corpo={'R$ 1,335.73'}
                    plus={true}
                    quantidade={'3.48%'}
                    periodo={'que ontem'}
                    icon={MdOutlineAttachMoney}
                />
                <SmallCard
                    titulo={'ACESSOS DIÁRIOS'}
                    corpo={'67'}
                    plus={true}
                    quantidade={'5.40%'}
                    periodo={'que ontem'}
                    icon={FaCar}
                />
                <SmallCard
                    titulo={'ACESSOS MENSAIS'}
                    corpo={'2,211'}
                    plus={false}
                    quantidade={'1.82%'}
                    periodo={'que mês passado'}
                    icon={MdExitToApp}
                />
                <SmallCard
                    titulo={'FATURAMENTO MENSAL'}
                    corpo={'R$ 43,144.76'}
                    plus={true}
                    quantidade={'104.76%'}
                    periodo={'que mês passado'}
                    icon={IoNewspaperSharp}
                />
            </div>

            <div className="flex flex-row justify-between gap-x-7">
                <BillingChart />
                <AccessChart />
            </div>
        </div>
    );
};

export default DashboardManager;
