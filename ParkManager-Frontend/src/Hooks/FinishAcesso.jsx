import { useState } from 'react';
import { API_BASE_URL } from '../util/Constants';
import { useAuth } from '../features/auth/AuthContext';
import axios from 'axios';

const useFinishAcesso = () => {
    const [loading, setLoading] = useState(true);
    const [erro, setErro] = useState(null);

    const { token } = useAuth();

    const finishEspecificAcesso = async (id) => {
        try {
            await axios.post(
                `${API_BASE_URL}/Acesso/finalizar/${id}`,
                {},
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );
        } catch (err) {
            setErro(err);
        } finally {
            setLoading(false);
        }
    };

    return { finishEspecificAcesso, loading, erro };
};

export default useFinishAcesso;
