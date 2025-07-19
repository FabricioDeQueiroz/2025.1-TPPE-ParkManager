const SmallCard = ({ corpo, entrada }) => {
    console.log('Corpo:', corpo);
    return (
        <div className="flex flex-row justify-start gap-x-7 bg-background-card-dashboard rounded-[20px] shadow-lg w-[637px] h-[115px]">
            <div className="flex flex-col p-5">
                <p className="text-xs font-bold mb-3 text-card-dashboard-capt-text">
                    TIPO DO ACESSO ATUAL
                </p>
                <p className="text-[34px] font-bold mb-3 text-card-dashboard-text">
                    {corpo === 0
                        ? 'Por Tempo'
                        : corpo === 1
                          ? 'Diária'
                          : corpo === 2
                            ? 'Mensal'
                            : corpo === 3
                              ? 'Evento'
                              : 'Desconhecido'}
                </p>
            </div>
            <div className="w-[1.5px] h-[80%] self-center bg-gradient-to-b from-card-dashboard-capt-text/70 via-card-dashboard-capt-text to-card-dashboard-capt-text/70"></div>
            <div className="flex flex-col p-5">
                <p className="text-xs font-bold mb-3 text-card-dashboard-capt-text">
                    DATA E HORÁRIO DE ENTRADA DO ACESSO ATUAL
                </p>
                <p className="text-[34px] font-bold mb-3 text-dashboard-tea">
                    {entrada}
                </p>
            </div>
        </div>
    );
};

export default SmallCard;
