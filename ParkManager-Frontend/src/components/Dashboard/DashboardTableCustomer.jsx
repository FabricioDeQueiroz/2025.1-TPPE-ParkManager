import { DateConverter } from '../../util/DateConverter';

export default function DashboardTableCustomer({ data, limit = 5 }) {
    const limitedData = data.slice(0, limit);

    return (
        <div className="bg-background-card-dashboard rounded-[20px] shadow-md w-[1589px] h-[350px]">
            <div className="flex flex-row justify-between items-center m-4">
                <p className="text-lg font-bold text-card-dashboard-text">
                    Sues acessos recentes
                </p>
                <button className="hover:cursor-pointer rounded-lg text-base text-dashboard-create-button-text bg-background-option-button-ac hover:bg-background-option-button-ac-hover font-bold w-[110px] h-[40px]">
                    VER TUDO
                </button>
            </div>

            <div className="overflow-x-auto">
                <table className="min-w-full text-left table-fixed">
                    <thead className="bg-card-dashboard-background-table text-card-dashboard-text-table uppercase tracking-wider">
                        <tr>
                            <th className="w-3/8 py-2.5 px-6 text-xs">
                                Nome do Estacionamento
                            </th>
                            <th className="w-1/8 py-2.5 px-6 text-xs">Placa</th>
                            <th className="w-1/8 py-2.5 px-6 text-xs">
                                Entrada
                            </th>
                            <th className="w-1/8 py-2.5 px-6 text-xs">Saída</th>
                            <th className="w-1/8 py-2.5 px-6 text-xs text-end">
                                Status
                            </th>
                        </tr>
                    </thead>
                    <tbody className="text-card-dashboard-text opacity-80 text-[15.5px]">
                        {limitedData.map((item, index) => (
                            <tr
                                key={index}
                                className="border-t border-card-dashboard-line-table"
                            >
                                <td className="w-3/8 py-3 px-6 truncate">
                                    {item.estacionamento.nome}
                                </td>
                                <td className="w-1/8 py-3 px-6">
                                    {item.placaVeiculo}
                                </td>
                                <td className="w-1/8 py-3 px-6 text-base mt-0.5">
                                    {DateConverter(
                                        item.dataHoraEntrada,
                                        true,
                                        2
                                    )}
                                </td>
                                <td className="w-1/8 py-3 px-6 text-base mt-0.5">
                                    {item.dataHoraSaida
                                        ? DateConverter(
                                              item.dataHoraSaida,
                                              true,
                                              2
                                          )
                                        : '-'}
                                </td>
                                <td className="w-1/8 py-3 px-6 text-base">
                                    <div className="flex justify-end">
                                        {item.dataHoraSaida ? (
                                            <div className="flex flex-row justify-center items-center bg-dashboard-green-500 w-36 h-6 rounded-md">
                                                <p className="text-text-page-title text-sm font-bold">
                                                    Encerrado
                                                </p>
                                            </div>
                                        ) : (
                                            <div className="flex flex-row justify-center items-center bg-dashboard-occupation w-36 h-6 rounded-md">
                                                <p className="text-text-page-title text-sm font-bold">
                                                    Estacionado
                                                </p>
                                            </div>
                                        )}
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
