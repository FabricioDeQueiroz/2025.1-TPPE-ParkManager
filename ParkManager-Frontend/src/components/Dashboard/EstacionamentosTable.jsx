import { useState } from 'react';
import { FaCirclePlus } from 'react-icons/fa6';
import { FaDoorOpen, FaDoorClosed } from 'react-icons/fa';
import { formatHour } from '../../util/DateConverter';
import { TbClock24 } from 'react-icons/tb';
import ActionButton from '../Form/ActionButton';
import { FaEye, FaPencil, FaTrash } from 'react-icons/fa6';
import ModalDelete from '../Modais/ModalDelete';
import useDeleteEstacionamento from '../../Hooks/DeleteEstacionamento';
import { showToast } from '../Feedback/ToastNotify';
import CreateEstacionamentoModal from '../Modais/CreateEstacionamentoModal';
import UpdateEstacionamentoModal from '../Modais/UpdateEstacionamentoModal';

export default function EstacionamentosTable({ data, limit = 6 }) {
    const [currentPage, setCurrentPage] = useState(1);
    const [selectedId, setSelectedId] = useState(null);

    const totalPages = Math.ceil(data.length / limit);
    const paginatedData = data.slice(
        (currentPage - 1) * limit,
        currentPage * limit
    );

    const { handleDelete, erro } = useDeleteEstacionamento();

    const handleDeleteClick = (id) => {
        setSelectedId(id);
        document.getElementById('delete-modal').showModal();
    };

    const handleUpdateClick = (id) => {
        setSelectedId(id);
        document.getElementById('update-modal').showModal();
    };

    return (
        <div className="bg-background-card-dashboard rounded-[20px] shadow-md w-[1589px] h-[600px]">
            <div className="flex flex-row justify-between items-center m-4">
                <p className="text-lg font-bold text-card-dashboard-text">
                    Estacionamentos Gerenciados
                </p>
                <button
                    onClick={() =>
                        document.getElementById('create-modal').showModal()
                    }
                    className="flex flex-row justify-between items-center px-4 hover:cursor-pointer rounded-lg text-base text-dashboard-create-button-text bg-dashboard-create-button hover:bg-dashboard-create-button/85 font-bold w-[310px] h-[39px]"
                >
                    <FaCirclePlus className="w-4 h-4 text-dashboard-create-button-text" />
                    <p>ADICIONAR ESTACIONAMENTO</p>
                </button>
            </div>

            <div className="overflow-x-auto relative h-[517px]">
                <table className="min-w-full text-left table-fixed">
                    <thead className="bg-card-dashboard-background-table text-card-dashboard-text-table uppercase tracking-wider">
                        <tr>
                            <th className="w-2/8 py-2.5 px-6 text-xs">
                                Estacionamento e Contratante
                            </th>
                            <th className="w-1/8 py-2.5 px-6 text-xs">
                                Horários
                            </th>
                            <th className="w-1/8 py-2.5 px-6 text-xs">
                                Ocupação
                            </th>
                            <th className="w-1/8 py-2.5 px-6 text-xs text-right"></th>
                        </tr>
                    </thead>
                    <tbody className="text-card-dashboard-text text-[15.5px]">
                        {paginatedData.map((item, index) => (
                            <tr
                                key={index}
                                className="border-t border-card-dashboard-line-table"
                            >
                                <td className="w-2/8 py-3 px-6 truncate opacity-80">
                                    <p className="font-bold">{item.nome}</p>
                                    <p>{item.nomeContratante}</p>
                                </td>
                                <td className="w-1/8 py-3 px-6 opacity-80">
                                    {item.tipo === 0 ? (
                                        <>
                                            <div className="flex flex-row gap-x-3 gap-y-1 items-center">
                                                <FaDoorOpen className="w-5 h-5 text-dashboard-green-500" />
                                                <p className="text-sm font-bold mt-0.5">
                                                    {formatHour(
                                                        item.horaAbertura
                                                    )}
                                                </p>
                                            </div>
                                            <div className="flex flex-row gap-x-3 gap-y-1 items-center">
                                                <FaDoorClosed className="w-5 h-5 text-dashboard-red-500" />
                                                <p className="text-sm font-bold mt-0.5">
                                                    {formatHour(
                                                        item.horaFechamento
                                                    )}
                                                </p>
                                            </div>
                                        </>
                                    ) : (
                                        <div className="flex flex-row gap-x-3 gap-y-1 items-center">
                                            <TbClock24 className="w-5 h-5 text-dashboard-tea" />
                                            <p className="text-sm font-bold mt-0.5">
                                                24h
                                            </p>
                                        </div>
                                    )}
                                </td>
                                <td className="w-1/8 py-3 px-6 text-base">
                                    <div className="flex flex-row gap-x-3 items-center justify-between bg-dashboard-occupation rounded-[6px] w-fit h-[25px] px-3">
                                        <p className="text-dashboard-create-button-text font-bold text-sm">
                                            {item.vagasOcupadas}
                                        </p>
                                        <div className="w-[1.5px] h-[55%] self-center bg-dashboard-create-button-text/70"></div>
                                        <p className="text-dashboard-create-button-text font-bold text-sm">
                                            {item.vagasTotais}
                                        </p>
                                    </div>
                                </td>
                                <td className="w-1/8 py-3 px-6">
                                    <div className="flex flex-row gap-x-8 justify-end">
                                        {/* <ActionButton
                                            icon={FaEye}
                                            label="VER"
                                            color="text-background-option-button-ac"
                                            action={() =>
                                                console.log('Ver detalhes')
                                            }
                                        /> */}
                                        <ActionButton
                                            icon={FaPencil}
                                            label="EDITAR"
                                            color="text-text-option-ac"
                                            action={() =>
                                                handleUpdateClick(
                                                    item.idEstacionamento
                                                )
                                            }
                                        />
                                        <ActionButton
                                            icon={FaTrash}
                                            label="DELETAR"
                                            color="text-dashboard-red-500"
                                            action={() =>
                                                handleDeleteClick(
                                                    item.idEstacionamento
                                                )
                                            }
                                        />
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                <div className="absolute bottom-0 left-0 w-full flex justify-center items-center mt-4 px-6 space-x-2">
                    <button
                        className={`px-3 font-bold w-[90px] py-1 rounded bg-dashboard-create-button-de ${currentPage === 1 ? 'text-dashboard-create-button/50 cursor-not-allowed' : 'text-dashboard-create-button hover:bg-dashboard-create-button/20'}`}
                        onClick={() =>
                            setCurrentPage((prev) => Math.max(prev - 1, 1))
                        }
                        disabled={currentPage === 1}
                    >
                        Anterior
                    </button>

                    <span className="w-[50px] py-1 rounded text-center font-bold bg-dashboard-create-button text-dashboard-create-button-text">
                        {currentPage}
                    </span>

                    <button
                        className={`px-3 font-bold w-[90px] py-1 rounded bg-dashboard-create-button-de ${currentPage === totalPages ? 'text-dashboard-create-button/50 cursor-not-allowed' : 'text-dashboard-create-button hover:bg-dashboard-create-button/20'}`}
                        onClick={() =>
                            setCurrentPage((prev) =>
                                Math.min(prev + 1, totalPages)
                            )
                        }
                        disabled={currentPage === totalPages}
                    >
                        Próxima
                    </button>
                </div>

                <ModalDelete
                    type="estacionamento"
                    action={() => {
                        handleDelete(selectedId);
                        document.getElementById('delete-modal').close();
                        if (erro) {
                            showToast({
                                message: 'Erro ao excluir estacionamento!',
                                type: 'error',
                                duration: 3000,
                            });
                        }
                        showToast({
                            message: 'Estacionamento excluído com sucesso!',
                            type: 'success',
                            duration: 3000,
                        });
                    }}
                />
                <CreateEstacionamentoModal />
                <UpdateEstacionamentoModal
                    estacionamentoData={data.find(
                        (item) => item.idEstacionamento === selectedId
                    )}
                />
            </div>
        </div>
    );
}
