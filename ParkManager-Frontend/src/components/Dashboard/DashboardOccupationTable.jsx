import { useNavigate } from 'react-router-dom';

export default function DashboardOccupationTable({ data, limit = 5 }) {
    const limitedData = data.slice(0, limit);

    const navigate = useNavigate();

    return (
        <div className="bg-background-card-dashboard rounded-[20px] shadow-md min-w-[510px] max-w-[510px] h-[350px]">
            <div className="flex flex-row justify-between items-center m-4">
                <p className="text-lg font-bold text-card-dashboard-text">
                    Ocupação
                </p>
                <button
                    onClick={() => navigate('/estacionamentos')}
                    className="hover:cursor-pointer rounded-lg text-base text-dashboard-create-button-text bg-background-option-button-ac hover:bg-background-option-button-ac-hover font-bold w-[110px] h-[40px]"
                >
                    VER TUDO
                </button>
            </div>

            <div className="overflow-x-auto">
                <table className="min-w-[510px] text-left table-fixed">
                    <thead className="w-[510px] bg-card-dashboard-background-table text-card-dashboard-text-table uppercase tracking-wider">
                        <tr>
                            <th className="max-w-[80px] py-2.5 px-6 text-xs">
                                Estacionamento
                            </th>

                            <th className="max-w-[35px] py-2.5 px-6 text-xs text-end">
                                Ocupação
                            </th>
                        </tr>
                    </thead>
                    <tbody className="max-w-[510px] text-card-dashboard-text-table font-medium text-[15px]">
                        {limitedData.map((item, index) => (
                            <tr
                                key={index}
                                className="border-t border-card-dashboard-line-table"
                            >
                                <td className="max-w-[50px] min-w-[50px] py-3 px-6 truncate">
                                    {item.nome}
                                </td>

                                <td className="max-w-[15px] min-w-[15px] px-4 py-3">
                                    {/* TODO aqui tem o card de ocupação */}
                                    {/* <div className="flex flex-row justify-center items-center bg-dashboard-occupation h-6 rounded-md">
                                        <p className="text-text-page-title text-sm font-bold">
                                            20
                                        </p>
                                        <vr className="border-l border-card-dashboard-line-table h-4 mx-2"></vr>
                                        <p className="text-text-page-title text-sm font-bold">
                                            400
                                        </p>
                                    </div> */}
                                    <progress
                                        className="progress text-dashboard-tea justify-center h-2 mt-3 bg-dashboard-occupation"
                                        value={item.vagasOcupadas}
                                        max={item.vagasTotais}
                                    ></progress>
                                    <div className="flex flex-row justify-between">
                                        <p className="text-sm ml-2 font-bold text-card-dashboard-text-table">
                                            {(
                                                (item.vagasOcupadas /
                                                    item.vagasTotais) *
                                                100
                                            )
                                                .toFixed(2)
                                                .replace('.', ',')}
                                            %
                                        </p>
                                        <p className="text-sm mr-2 text-card-dashboard-text-table">
                                            {item.vagasOcupadas} /{' '}
                                            {item.vagasTotais}
                                        </p>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
