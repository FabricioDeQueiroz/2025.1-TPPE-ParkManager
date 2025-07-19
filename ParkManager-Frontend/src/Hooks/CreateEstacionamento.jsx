import { useState } from 'react';
import { API_BASE_URL } from '../util/Constants';
import { useAuth } from '../features/auth/AuthContext';
import axios from 'axios';

const useCreateEstacionamento = () => {
    const [loading, setLoading] = useState(false);
    const [erro, setErro] = useState(null);

    const { token } = useAuth();

    const handleCreate = async (data) => {
        setLoading(true);
        try {
            await axios.post(`${API_BASE_URL}/Estacionamento`, data, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            window.location.reload();
        } catch (err) {
            setErro(err);
        } finally {
            setLoading(false);
        }
    };

    return { handleCreate, loading, erro };
};

export default useCreateEstacionamento;
