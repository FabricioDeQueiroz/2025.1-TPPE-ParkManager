import { useState } from 'react';
import { API_BASE_URL } from '../util/Constants';
import { useAuth } from '../features/auth/AuthContext';
import axios from 'axios';

const updateEstacionamento = () => {
    const [loading, setLoading] = useState(false);
    const [erro, setErro] = useState(null);

    const { token } = useAuth();

    const handleUpdate = async (data) => {
        setLoading(true);
        try {
            await axios.put(
                `${API_BASE_URL}/Estacionamento/${data.idEstacionamento}`,
                data,
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );

            window.location.reload();
        } catch (err) {
            setErro(err);
        } finally {
            setLoading(false);
        }
    };

    return { handleUpdate, loading, erro };
};

export default updateEstacionamento;
