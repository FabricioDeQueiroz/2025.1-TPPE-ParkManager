import { FaUserCircle, FaKey } from 'react-icons/fa';
import logo from '../../../public/logo.svg';
import NavigationLink from './NavigationLink';

const Navbar = () => {
    return (
        <nav className="flex justify-between mx-16 mt-5">
            <div className="flex items-center">
                <img src={logo} className="w-6" alt="Logo" />
                <p className="text-texto-branco text-sm font-bold ml-3">
                    ParkManager
                </p>
            </div>

            <div className="flex items-center gap-x-8">
                <NavigationLink
                    icon={FaUserCircle}
                    label="CADASTRO"
                    to="/cadastro"
                />
                <NavigationLink icon={FaKey} label="ENTRAR" to="/" />
            </div>
        </nav>
    );
};

export default Navbar;
