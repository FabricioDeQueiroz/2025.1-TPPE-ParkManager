import Sidebar from '../components/Sidebar/Sidebar';
import Breadcrumb from '../components/NavBar/Breadcrumb';
import AcessosTable from '../components/Dashboard/AcessosTable';
import getAcessos from '../Hooks/GetAcessos';

const Acessos = () => {
    const { acessos, loading, erro } = getAcessos();

    if (loading) {
        return (
            <div className="flex flex-row">
                <Sidebar />

                <div className="flex flex-col h-[calc(100vh-52px)] max-w-[calc(100vh-256px)] min-w-fit mx-5 mb-5 mt-8">
                    <div className="mb-2">
                        <Breadcrumb
                            lista={['ParkManager', 'Acessos']}
                            caminhos={['', '/acessos']}
                        />
                    </div>

                    <div className="text-xl font-bold mb-2 text-text-page-title">
                        Acessos
                    </div>

                    <div className="flex flex-row justify-center items-center w-[1580px]">
                        <span className="loading loading-ring w-3xl text-dashboard-green-400"></span>
                    </div>
                </div>
            </div>
        );
    }

    if (erro) {
        return (
            <div className="flex flex-row">
                <Sidebar />

                <div className="flex flex-col h-[calc(100vh-52px)] max-w-[calc(100vh-256px)] min-w-fit mx-5 mb-5 mt-8">
                    <div className="mb-2">
                        <Breadcrumb
                            lista={['ParkManager', 'Acessos']}
                            caminhos={['', '/acessos']}
                        />
                    </div>

                    <div className="text-xl font-bold mb-2 text-text-page-title">
                        Acessos
                    </div>

                    <div className="w-[1580px] mt-8">
                        <span className="text-dashboard-red-400 text-2xl font-bold">
                            {erro}
                        </span>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="flex flex-row">
            <Sidebar />

            <div className="flex flex-col h-[calc(100vh-52px)] max-w-[calc(100vh-256px)] min-w-fit mx-5 mb-5 mt-8">
                <div className="mb-2">
                    <Breadcrumb
                        lista={['ParkManager', 'Acessos']}
                        caminhos={['', '/acessos']}
                    />
                </div>

                <div className="text-xl font-bold mb-2 text-text-page-title">
                    Acessos
                </div>

                <div className="flex flex-row justify-between gap-x-7 mb-5">
                    <AcessosTable data={acessos} />
                </div>
            </div>
        </div>
    );
};

export default Acessos;
