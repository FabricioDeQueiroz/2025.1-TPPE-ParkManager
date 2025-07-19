import { useState, useEffect } from 'react';
import useUpdateEstacionamento from '../../Hooks/UpdateEstacionamento';
import { useAuth } from '../../features/auth/AuthContext';

const UpdateEstacionamentoModal = ({ estacionamentoData }) => {
    const { user, userId } = useAuth();

    const [formData, setFormData] = useState({
        idEstacionamento: '',
        nome: '',
        nomeContratante: '',
        vagasTotais: '',
        vagasOcupadas: 0,
        faturamento: 0,
        retornoContratante: '',
        valorFracao: '',
        descontoHora: '',
        valorMensal: '',
        valorDiaria: '',
        adicionalNoturno: '',
        horaAbertura: '',
        horaFechamento: '',
        tipo: 0,
        is24h: false,
        idGerente: '',
    });

    const { handleUpdate } = useUpdateEstacionamento();

    useEffect(() => {
        if (estacionamentoData) {
            setFormData({
                ...estacionamentoData,
                retornoContratante: estacionamentoData.retornoContratante * 100,
                is24h: estacionamentoData.tipo === 1,
            });
        }
    }, [estacionamentoData]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const onSubmit = async (e) => {
        e.preventDefault();

        const is24h = formData.is24h;

        const payload = {
            idEstacionamento: formData.idEstacionamento || '',
            nome: formData.nome || '',
            nomeContratante: formData.nomeContratante || '',
            vagasTotais: Number(formData.vagasTotais) || 0,
            vagasOcupadas: Number(formData.vagasOcupadas) || 0,
            faturamento: Number(formData.faturamento) || 0,
            retornoContratante:
                formData.retornoContratante !== ''
                    ? parseFloat(formData.retornoContratante) / 100
                    : 0.01,
            valorFracao: Number(formData.valorFracao) || 0,
            descontoHora: Number(formData.descontoHora) || 0,
            valorMensal: Number(formData.valorMensal) || 0,
            valorDiaria: Number(formData.valorDiaria) || 0,
            adicionalNoturno: Number(formData.adicionalNoturno) || 0,
            horaAbertura: is24h ? undefined : formData.horaAbertura || '',
            horaFechamento: is24h ? undefined : formData.horaFechamento || '',
            tipo: is24h ? 1 : 0,
            idGerente: userId || user.id || '',
        };

        try {
            await handleUpdate(payload);
            document.getElementById('update-modal').close();
        } catch (error) {
            console.error('Erro ao atualizar estacionamento:', error);
        }
    };

    return (
        <dialog id="update-modal" className="modal">
            <div className="modal-box bg-background-card-dashboard w-[720px] min-w-[720px] rounded-[10px] shadow-xl p-10 relative">
                <form method="dialog">
                    <button className="hover:cursor-pointer absolute top-4 right-4 text-2xl text-card-dashboard-text">
                        ✕
                    </button>
                </form>

                <h2 className="text-xl font-bold text-card-dashboard-text text-center mb-6">
                    Atualize os dados do estacionamento:
                </h2>

                <div className="grid grid-cols-1 gap-4 mb-4">
                    <input
                        required={true}
                        name="nome"
                        value={formData.nome}
                        onChange={handleChange}
                        type="text"
                        placeholder="Nome"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        name="nomeContratante"
                        value={formData.nomeContratante}
                        onChange={handleChange}
                        type="text"
                        placeholder="Nome do contratante"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        name="vagasTotais"
                        value={formData.vagasTotais}
                        onChange={handleChange}
                        type="number"
                        placeholder="Capacidade máxima"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <div className="flex justify-between items-center w-full">
                        <div className="gap-2 flex items-center">
                            <input
                                required={true}
                                value={formData.is24h}
                                type="checkbox"
                                id="is24h"
                                checked={formData.is24h}
                                onChange={() =>
                                    setFormData((prev) => ({
                                        ...prev,
                                        is24h: !prev.is24h,
                                    }))
                                }
                            />
                            <label
                                htmlFor="is24h"
                                className="text-card-dashboard-capt-text"
                            >
                                É 24h
                            </label>
                        </div>
                        <input
                            required={true}
                            name="horaAbertura"
                            value={formData.horaAbertura}
                            onChange={handleChange}
                            type="time"
                            placeholder="Abre às..."
                            disabled={formData.is24h}
                            className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-[42%] pr-2 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active disabled:opacity-50 [&::-webkit-calendar-picker-indicator]:invert"
                        />
                        <input
                            required={true}
                            name="horaFechamento"
                            value={formData.horaFechamento}
                            onChange={handleChange}
                            type="time"
                            placeholder="Fecha às..."
                            disabled={formData.is24h}
                            className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-[42%] pr-2 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active disabled:opacity-50 [&::-webkit-calendar-picker-indicator]:invert"
                        />
                    </div>
                </div>

                <h2 className="text-xl font-bold text-card-dashboard-text text-center mb-6 mt-14">
                    Atualize os valores cobrados pelo estacionamento:
                </h2>

                <div className="grid grid-cols-2 gap-4 mb-14">
                    <input
                        required={true}
                        name="valorFracao"
                        value={formData.valorFracao}
                        onChange={handleChange}
                        type="number"
                        placeholder="Valor fração (15min)"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        name="descontoHora"
                        value={formData.descontoHora}
                        onChange={handleChange}
                        type="number"
                        placeholder="Desconto hora"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        name="valorDiaria"
                        value={formData.valorDiaria}
                        onChange={handleChange}
                        type="number"
                        placeholder="Valor diária"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        name="adicionalNoturno"
                        value={formData.adicionalNoturno}
                        onChange={handleChange}
                        type="number"
                        placeholder="Adicional noturno"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        name="valorMensal"
                        value={formData.valorMensal}
                        onChange={handleChange}
                        type="number"
                        placeholder="Valor mensal"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        name="retornoContratante"
                        value={formData.retornoContratante}
                        onChange={handleChange}
                        type="number"
                        placeholder="Percentual de retorno ao contratante (%)"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active col-span-2"
                    />
                </div>

                <button
                    onClick={onSubmit}
                    className="hover:cursor-pointer w-full h-[60px] font-bold text-white text-[20px] bg-dashboard-create-button rounded-lg hover:opacity-90"
                >
                    ATUALIZAR
                </button>
            </div>
        </dialog>
    );
};

export default UpdateEstacionamentoModal;
