import { useNavigate } from 'react-router-dom';

const SmallCardCustomer = ({
    titulo,
    corpo,
    icon: Icon,
    width = 'w-[385px]',
    widthCorpo,
    button,
    to,
    fontSize,
}) => {
    const navigate = useNavigate();

    return (
        <div
            className={`flex flex-row justify-between p-5 bg-background-card-dashboard rounded-[20px] shadow-lg ${width} h-[115px]`}
        >
            <div className="flex flex-col">
                <p className="text-xs font-bold mb-3 text-card-dashboard-capt-text">
                    {titulo}
                </p>
                <p
                    className={`font-bold text-card-dashboard-text line-clamp-2 ${widthCorpo ? 'leading-6' : ''} ${widthCorpo} ${fontSize}`}
                >
                    {corpo}
                </p>
            </div>
            {button ? (
                <div className="flex flex-col">
                    <div
                        onClick={() => navigate(to)}
                        className="bg-background-option-button-ac hover:bg-background-option-button-ac-hover cursor-pointer w-12 h-12 rounded-lg flex items-center justify-center"
                    >
                        <Icon className="text-card-dashboard-button-icon w-8 h-8" />
                    </div>
                </div>
            ) : (
                <div className="flex flex-col">
                    <div className="bg-card-dashboard-button w-12 h-12 rounded-lg flex items-center justify-center">
                        <Icon className="text-card-dashboard-button-icon w-8 h-8" />
                    </div>
                </div>
            )}
        </div>
    );
};

export default SmallCardCustomer;
