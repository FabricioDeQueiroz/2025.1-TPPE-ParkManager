import { useAuth } from '../../features/auth/AuthContext';
import logo from '../../../public/logo.svg';
import { FaHome, FaCalendarAlt, FaMoon } from 'react-icons/fa';
import { FaSquareParking } from 'react-icons/fa6';
import { SidebarButton } from './SidebarButton';
import { RxEnter } from 'react-icons/rx';
import { ThemeToggleButton } from './ThemeToggleButton';
import { LogoutButton } from './LogoutButton';

const Sidebar = () => {
    const { userName, userType, userEmail } = useAuth();

    return (
        <div className="bg-background-navbar flex flex-col justify-between h-[calc(100vh-52px)] w-64 ml-5 mb-5 mt-8 rounded-[10px] shadow-lg">
            <div>
                <div className="flex items-center ml-6 mt-6 mb-6">
                    <img src={logo} className="w-8" alt="Logo" />
                    <p className="text-text-logo font-bold ml-3">ParkManager</p>
                </div>

                <div className="h-[2px] my-2 bg-gradient-to-r from-background-option-ac via-line-navbar to-background-option-ac" />

                <div className="flex flex-col items-center bg-background-option-ac rounded-lg p-2.5 my-3 mx-3">
                    <div className="avatar">
                        <div className="w-20 rounded-full">
                            <img
                                src={
                                    userEmail ===
                                    'fabriciodequeiroz2016@gmail.com'
                                        ? 'https://img.itch.zone/aW1nLzE0MzI2MTEzLnBuZw==/original/fxq%2F48.png'
                                        : 'https://upload.wikimedia.org/wikipedia/commons/7/7c/Profile_avatar_placeholder_large.png?20150327203541'
                                }
                            />
                        </div>
                    </div>
                    <p className="text-text-user-name mt-2.5 mb-1.5">
                        {userName}
                    </p>
                    <p className="text-text-user-role font-semibold">
                        {userType == 0 ? 'Gerente' : 'Cliente'}
                    </p>
                </div>

                <div className="h-[2px] mb-6 bg-gradient-to-r from-background-option-ac via-line-navbar to-background-option-ac" />

                {/* Apresenta apenas os botões corretos para o cargo */}
                {userType === 0 ? (
                    <>
                        <SidebarButton
                            icon={FaHome}
                            label="Painel"
                            to="/parkmanager/dashboard"
                        />
                        <SidebarButton
                            icon={FaSquareParking}
                            label="Estacionamentos"
                            to="/parkmanager/estacionamentos"
                        />
                        <SidebarButton
                            icon={FaCalendarAlt}
                            label="Eventos"
                            to="/parkmanager/eventos"
                        />
                    </>
                ) : (
                    <>
                        <SidebarButton
                            icon={FaHome}
                            label="Painel"
                            to="/parkmanager/dashboard"
                        />
                        <SidebarButton
                            icon={RxEnter}
                            label="Acessos"
                            to="/parkmanager/acessos"
                        />
                    </>
                )}

                <div className="h-[2px] mb-6 bg-gradient-to-r from-background-option-ac via-line-navbar to-background-option-ac" />
            </div>
            <div>
                <ThemeToggleButton />

                <LogoutButton />
            </div>
        </div>
    );
};

export default Sidebar;
