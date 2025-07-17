import { ImExit } from 'react-icons/im';
import { useAuth } from '../../features/auth/AuthContext';
import { Navigate } from 'react-router-dom';

export const LogoutButton = () => {
    const { logout } = useAuth();

    const handleLogout = async () => {
        await logout();
        return <Navigate to="/" replace />;
    };

    return (
        <div
            onClick={() => handleLogout()}
            className="group flex flex-row items-center bg-background-option-de rounded-lg py-3 px-4 mb-4 mx-3 hover:bg-background-option-button-ac cursor-pointer border-2 border-background-option-button-de"
        >
            <div className="bg-background-option-button-de w-9 h-9 rounded-lg mr-3 flex items-center justify-center group-hover:bg-background-option-icon-de">
                <ImExit className="text-background-option-icon-de w-6 h-6 group-hover:text-background-option-icon-ac" />
            </div>
            <p className="text-text-option-de font-bold group-hover:text-background-option-icon-ac">
                Sair
            </p>
        </div>
    );
};
