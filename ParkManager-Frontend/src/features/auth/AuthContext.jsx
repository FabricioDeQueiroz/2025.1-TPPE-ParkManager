import { createContext, useContext, useEffect, useState } from 'react';
import { API_BASE_URL } from '../../util/Constants';
import { Navigate } from 'react-router-dom';
import axios from 'axios';
import getFirstValidationError from '../../util/FirstElementJson';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [token, setToken] = useState(null);
    const [user, setUser] = useState(null);
    const [userId, setUserId] = useState(null);
    const [userName, setUserName] = useState(null);
    const [userEmail, setUserEmail] = useState(null);
    const [userType, setUserType] = useState(null);
    const [isReady, setIsReady] = useState(false);

    const login = async (userData) => {
        try {
            const res = await axios.post(
                `${API_BASE_URL}/Usuario/login`,
                userData
            );

            setToken(res.data.token);
            setUserId(res.data.id);
            setUserName(res.data.nome);
            setUserEmail(res.data.email);
            setUserType(res.data.tipo);
            setUser(res.data);

            localStorage.setItem(
                'parkmanager@auth_user',
                JSON.stringify(res.data)
            );
            localStorage.setItem('parkmanager@auth_token', res.data.token);

            setIsReady(true);
            return {
                passou: true,
                retorno: res,
            };
        } catch (error) {
            setIsReady(true);

            return {
                passou: false,
                retorno:
                    error.response?.data.message ||
                    getFirstValidationError(error.response?.data.errors) ||
                    'Erro ao fazer login',
            };
        }
    };

    const register = async (userData) => {
        try {
            await axios.post(`${API_BASE_URL}/Usuario/register`, userData);

            return {
                passou: true,
            };
        } catch (error) {
            return {
                passou: false,
                retorno:
                    error.response?.data.message ||
                    getFirstValidationError(error.response?.data.errors) ||
                    error.response.data ||
                    'Erro ao fazer cadastro',
            };
        }
    };

    const logout = async () => {
        setToken(null);
        setUserId(null);
        setUserName(null);
        setUserEmail(null);
        setUserType(null);
        setUser(null);

        localStorage.removeItem('parkmanager@auth_user');
        localStorage.removeItem('parkmanager@auth_token');

        return <Navigate to="/parkmanager/" />;
    };

    const retrive_user_data = async () => {
        const user = JSON.parse(localStorage.getItem('parkmanager@auth_user'));
        const token = localStorage.getItem('parkmanager@auth_token');

        if (user && token) {
            setUserId(user.id);
            setUserName(user.nome);
            setUserEmail(user.email);
            setUserType(user.tipo);
            setToken(token);
            setIsReady(true);
            setUser(user);
            return;
        }

        setIsReady(true);
    };

    useEffect(() => {
        retrive_user_data();
    }, []);

    return (
        <AuthContext.Provider
            value={{
                userId,
                userName,
                userEmail,
                userType,
                token,
                user,
                login,
                register,
                logout,
            }}
        >
            {isReady ? children : null}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);

export default AuthContext;
