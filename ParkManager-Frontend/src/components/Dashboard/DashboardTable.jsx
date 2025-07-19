import { DateConverter } from '../../util/DateConverter';
import { useNavigate } from 'react-router-dom';

export default function DashboardTable({ data, limit = 5 }) {
    const limitedData = data.slice(0, limit);

    const navigate = useNavigate();

    return (
        <div className="bg-background-card-dashboard rounded-[20px] shadow-md w-[1050px] h-[350px]">
            <div className="flex flex-row justify-between items-center m-4">
                <p className="text-lg font-bold text-card-dashboard-text">
                    Acessos recentes
                </p>
                <button
                    onClick={() => navigate('/parkmanager/estacionamentos')}
                    className="hover:cursor-pointer rounded-lg text-base text-dashboard-create-button-text bg-background-option-button-ac hover:bg-background-option-button-ac-hover font-bold w-[110px] h-[40px]"
                >
                    VER TUDO
                </button>
            </div>

            <div className="overflow-x-auto">
                <table className="min-w-full text-left table-fixed">
                    <thead className="bg-card-dashboard-background-table text-card-dashboard-text-table uppercase tracking-wider">
                        <tr>
                            <th className="w-5/8 py-2.5 px-6 text-xs">
                                Nome do Estacionamento
                            </th>
                            <th className="w-1/8 py-2.5 px-6 text-xs">Placa</th>
                            <th className="w-2/8 py-2.5 px-6 text-xs text-end">
                                Entrada
                            </th>
                        </tr>
                    </thead>
                    <tbody className="text-card-dashboard-text opacity-80 text-[15.5px]">
                        {limitedData.map((item, index) => (
                            <tr
                                key={index}
                                className="border-t border-card-dashboard-line-table"
                            >
                                <td className="w-5/8 py-3 px-6 truncate">
                                    {item.estacionamento.nome}
                                </td>
                                <td className="w-1/8 py-3 px-6">
                                    {item.placaVeiculo}
                                </td>
                                <td className="w-2/8 py-3 px-6 text-end text-base mt-0.5">
                                    {DateConverter(
                                        item.dataHoraEntrada,
                                        true,
                                        2
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
