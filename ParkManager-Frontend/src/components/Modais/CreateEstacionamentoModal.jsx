import { useState } from 'react';
import useCreateEstacionamento from '../../Hooks/CreateEstacionamento';

const CreateEstacionamentoModal = () => {
    const [formData, setFormData] = useState({
        nome: '',
        contratante: '',
        capacidade: '',
        is24h: false,
        abreAs: '',
        fechaAs: '',
        valorFracao: '',
        descontoHora: '',
        valorDiaria: '',
        adicionalNoturno: '',
        valorMensal: '',
        valorEvento: '',
        percentualRetorno: '',
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        onSubmit(formData);
        setFormData({
            nome: '',
            contratante: '',
            capacidade: '',
            is24h: false,
            abreAs: '',
            fechaAs: '',
            valorFracao: '',
            descontoHora: '',
            valorDiaria: '',
            adicionalNoturno: '',
            valorMensal: '',
            valorEvento: '',
            percentualRetorno: '',
        });
    };

    const { handleCreate, loading, erro } = useCreateEstacionamento();

    const onSubmit = async (e) => {
        e.preventDefault();
        try {
            await handleCreate(formData);
            document.getElementById('create-modal').close();
        } catch (error) {
            console.error('Erro ao criar estacionamento:', error);
        }
    };

    return (
        <dialog id="create-modal" className="modal">
            <div className="modal-box bg-background-card-dashboard w-[720px] min-w-[720px] rounded-[10px] shadow-xl p-10 relative">
                <form method="dialog">
                    <button className="hover:cursor-pointer absolute top-4 right-4 text-2xl text-card-dashboard-text">
                        ✕
                    </button>
                </form>

                <h2 className="text-xl font-bold text-card-dashboard-text text-center mb-6">
                    Digite os dados do estacionamento a ser cadastrado:
                </h2>

                <div className="grid grid-cols-1 gap-4 mb-4">
                    <input
                        required={true}
                        value={formData.nome}
                        onChange={handleChange}
                        type="text"
                        placeholder="Nome"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        value={formData.contratante}
                        onChange={handleChange}
                        type="text"
                        placeholder="Nome do contratante"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        value={formData.capacidade}
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
                            value={formData.abreAs}
                            onChange={handleChange}
                            type="time"
                            placeholder="Abre às..."
                            disabled={formData.is24h}
                            className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-[42%] pr-2 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active disabled:opacity-50 [&::-webkit-calendar-picker-indicator]:invert"
                        />
                        <input
                            required={true}
                            value={formData.fechaAs}
                            onChange={handleChange}
                            type="time"
                            placeholder="Fecha às..."
                            disabled={formData.is24h}
                            className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-[42%] pr-2 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active disabled:opacity-50 [&::-webkit-calendar-picker-indicator]:invert"
                        />
                    </div>
                </div>

                <h2 className="text-xl font-bold text-card-dashboard-text text-center mb-6 mt-14">
                    Digite os valores cobrados pelo estacionamento:
                </h2>

                <div className="grid grid-cols-2 gap-4 mb-14">
                    <input
                        required={true}
                        value={formData.valorFracao}
                        onChange={handleChange}
                        type="number"
                        placeholder="Valor fração (15min)"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        value={formData.descontoHora}
                        onChange={handleChange}
                        type="number"
                        placeholder="Desconto hora"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        value={formData.valorDiaria}
                        onChange={handleChange}
                        type="number"
                        placeholder="Valor diária"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        value={formData.adicionalNoturno}
                        onChange={handleChange}
                        type="number"
                        placeholder="Adicional noturno"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        value={formData.valorMensal}
                        onChange={handleChange}
                        type="number"
                        placeholder="Valor mensal"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        value={formData.valorEvento}
                        onChange={handleChange}
                        type="number"
                        placeholder="Valor evento"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />
                    <input
                        required={true}
                        value={formData.percentualRetorno}
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
                    CADASTRAR
                </button>
            </div>
        </dialog>
    );
};

export default CreateEstacionamentoModal;
