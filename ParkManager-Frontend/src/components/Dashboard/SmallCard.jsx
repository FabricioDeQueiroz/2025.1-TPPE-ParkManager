const SmallCard = ({
    titulo,
    corpo,
    plus,
    signal,
    quantidade,
    periodo,
    icon: Icon,
}) => {
    return (
        <div className="flex flex-row justify-between p-5 bg-background-card-dashboard rounded-[20px] shadow-lg w-[375px] h-[115px]">
            <div className="flex flex-col">
                <p className="text-xs font-bold mb-1 text-card-dashboard-capt-text">
                    {titulo}
                </p>
                <p className="text-lg font-bold mb-3 text-card-dashboard-text">
                    {corpo}
                </p>
                <span className="text-sm flex items-center gap-x-1.5">
                    {plus ? (
                        <p className="text-dashboard-green-500 font-bold">
                            {signal}
                            {quantidade}
                        </p>
                    ) : (
                        <p className="text-dashboard-red-500 font-bold">
                            {signal}
                            {quantidade}
                        </p>
                    )}
                    <p className="text-card-dashboard-footer-text">{periodo}</p>
                </span>
            </div>
            <div className="flex flex-col">
                <div className="bg-card-dashboard-button w-12 h-12 rounded-lg flex items-center justify-center">
                    {Icon && (
                        <Icon className="text-card-dashboard-button-icon w-6 h-6" />
                    )}
                </div>
            </div>
        </div>
    );
};

export default SmallCard;
