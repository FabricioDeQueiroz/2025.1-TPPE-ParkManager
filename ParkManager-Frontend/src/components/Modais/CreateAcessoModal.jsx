import { useState } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../../util/Constants';
import { useAuth } from '../../features/auth/AuthContext';
import getEstacionamentos from '../../Hooks/GetEstacionamentos';
import { showToast } from '../Feedback/ToastNotify';

const CreateAcessoModal = () => {
    const [formData, setFormData] = useState({
        placaVeiculo: '',
        idEstacionamento: '',
    });

    const { estacionamentos } = getEstacionamentos();

    const { token } = useAuth();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await axios.post(`${API_BASE_URL}/Acesso`, formData, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
            document.getElementById('create-acesso-modal').close();
            setFormData({ placaVeiculo: '', idEstacionamento: '' });
            showToast({
                message: 'Acesso realizado com sucesso!',
                type: 'success',
                duration: 1000,
                onClose: () => {
                    window.location.reload();
                },
            });
        } catch (error) {
            console.error('Erro ao criar acesso:', error);
            showToast({
                message: 'Erro ao realizar acesso!',
                type: 'error',
                duration: 1000,
                onClose: () => {
                    window.location.reload();
                },
            });
        }
    };

    return (
        <dialog id="create-acesso-modal" className="modal">
            <div className="modal-box bg-background-card-dashboard w-[520px] rounded-[10px] shadow-xl p-8 relative">
                <form method="dialog">
                    <button className="absolute top-4 right-4 text-2xl text-card-dashboard-text">
                        ✕
                    </button>
                </form>

                <h2 className="text-xl font-bold text-card-dashboard-text text-center mb-6">
                    Realizar novo acesso
                </h2>

                <div className="grid grid-cols-1 gap-4 mb-6">
                    <input
                        required
                        name="placaVeiculo"
                        value={formData.placaVeiculo}
                        onChange={handleChange}
                        type="text"
                        placeholder="Placa do veículo"
                        className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-4 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    />

                    <select
                        required
                        name="idEstacionamento"
                        value={formData.idEstacionamento}
                        onChange={handleChange}
                        className="transition duration-300 h-12 w-full pr-4 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                    >
                        <option
                            value=""
                            disabled
                            className="text-gray-400 bg-white hover:bg-gray-100"
                        >
                            Selecione um estacionamento
                        </option>
                        {estacionamentos.map((est) => (
                            <option
                                key={est.idEstacionamento}
                                value={est.idEstacionamento}
                                className="text-black bg-white hover:bg-gray-100 cursor-pointer"
                            >
                                {est.nome}
                            </option>
                        ))}
                    </select>
                </div>

                <button
                    onClick={handleSubmit}
                    className="hover:cursor-pointer w-full h-[50px] font-bold text-white text-[18px] bg-dashboard-create-button rounded-lg hover:opacity-90"
                >
                    CONFIRMAR ACESSO
                </button>
            </div>
        </dialog>
    );
};

export default CreateAcessoModal;
