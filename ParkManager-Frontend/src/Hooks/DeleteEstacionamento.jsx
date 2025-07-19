import { useState } from 'react';
import { API_BASE_URL } from '../util/Constants';
import { useAuth } from '../features/auth/AuthContext';
import axios from 'axios';

const deleteEstacionamento = () => {
    const [loading, setLoading] = useState(false);
    const [erro, setErro] = useState(null);

    const { token } = useAuth();

    const handleDelete = async (id) => {
        setLoading(true);
        try {
            await axios.delete(`${API_BASE_URL}/Estacionamento/${id}`, {
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

    return { handleDelete, loading, erro };
};

export default deleteEstacionamento;
