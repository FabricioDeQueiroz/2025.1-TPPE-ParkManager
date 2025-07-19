import { useAuth } from '../features/auth/AuthContext';
import { Navigate, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import Navbar from '../components/NavBar/LoginRegisterNavBar';
import InputField from '../components/Form/InputField';
import LoginRegisterButton from '../components/Form/LoginRegisterButton';

const Login = () => {
    const [email, setEmail] = useState('');
    const [senha, setSenha] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const navigate = useNavigate();

    const { token, login } = useAuth();

    if (token) {
        return <Navigate to="/dashboard" replace />;
    }

    const handleLogin = async (e) => {
        e.preventDefault();
        if (!email || !senha) {
            setError('Preencha todos os campos!');
            return;
        }
        setIsLoading(true);
        const logar = await login({ email, senha });
        if (logar.passou) {
            navigate('/dashboard', { replace: true });
        }
        setIsLoading(false);
        setError(`${logar.retorno}`);
    };

    return (
        <div>
            {
                <>
                    <div className="bg-gradient-to-b from-background-escuro to-background-claro flex flex-col h-screen w-screen">
                        <Navbar />
                        <div className="my-auto w-[452px] rounded-[20px] bg-background-login mx-auto self-center p-9">
                            <form
                                className="flex flex-col"
                                onSubmit={handleLogin}
                            >
                                <p className="text-3xl font-bold text-titulo-login mb-8 mx-auto self-center">
                                    Entrar
                                </p>
                                <div className="gap-y-6 flex flex-col mb-2">
                                    <InputField
                                        label="Email"
                                        type="email"
                                        value={email}
                                        onChange={(e) =>
                                            setEmail(e.target.value)
                                        }
                                        placeholder="Seu endereço de email"
                                        required={true}
                                    />

                                    <InputField
                                        label="Senha"
                                        type="password"
                                        value={senha}
                                        onChange={(e) =>
                                            setSenha(e.target.value)
                                        }
                                        placeholder="Sua senha"
                                        required={true}
                                    />
                                </div>

                                <div className="flex items-center justify-start">
                                    {error !== '' ? (
                                        <p className="text-dashboard-red-400 text-xs mb-6">
                                            {error}
                                        </p>
                                    ) : (
                                        <div className="mb-8"></div>
                                    )}
                                </div>

                                <LoginRegisterButton
                                    label="ENTRAR"
                                    isLoading={isLoading}
                                />

                                <span className="mt-4 mb-2 self-start text-[14px] flex items-center gap-x-2">
                                    <p className="text-text-register">
                                        Ainda não tem uma conta?
                                    </p>
                                    <p
                                        onClick={() => navigate('/cadastro')}
                                        className="text-button-register hover:underline hover:cursor-pointer font-bold"
                                    >
                                        Cadastre-se
                                    </p>
                                </span>
                            </form>
                        </div>
                    </div>
                </>
            }
        </div>
    );
};

export default Login;
