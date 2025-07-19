import { useEffect, useState } from 'react';
import { API_BASE_URL } from '../util/Constants';
import { useAuth } from '../features/auth/AuthContext';
import axios from 'axios';

const getEstacionamentos = () => {
    const [estacionamentos, setEstacionamentos] = useState([]);
    const [loading, setLoading] = useState(true);
    const [erro, setErro] = useState(null);

    const { token } = useAuth();

    useEffect(() => {
        const fetchEstacionamentos = async () => {
            try {
                const response = await axios.get(
                    `${API_BASE_URL}/Estacionamento`,
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );
                setEstacionamentos(response.data);
            } catch (err) {
                setErro(err);
            } finally {
                setLoading(false);
            }
        };

        fetchEstacionamentos();
    }, []);

    return { estacionamentos, loading, erro };
};

export default getEstacionamentos;
