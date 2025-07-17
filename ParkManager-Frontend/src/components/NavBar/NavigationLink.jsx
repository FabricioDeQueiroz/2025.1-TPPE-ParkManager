import { useNavigate } from 'react-router-dom';

const NavigationLink = ({ icon: Icon, label, to }) => {
    const navigate = useNavigate();

    return (
        <div
            className="flex items-center hover:cursor-pointer hover:underline"
            onClick={() => navigate(to)}
        >
            <Icon className="text-icon-branco w-3.5" />
            <p className="text-texto-branco text-xs font-bold ml-3">{label}</p>
        </div>
    );
};

export default NavigationLink;
