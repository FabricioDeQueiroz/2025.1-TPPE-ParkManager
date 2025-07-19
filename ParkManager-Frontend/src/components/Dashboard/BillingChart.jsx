import {
    AreaChart,
    Area,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
} from 'recharts';

const CustomTooltip = ({ active, payload, label }) => {
    if (active && payload && payload.length) {
        return (
            <div className="p-4 bg-background-card-dashboard border border-card-dashboard-capt-text rounded-md shadow-lg">
                <p className="font-bold text-card-dashboard-text">{`Mês: ${label == 'Jan' ? 'Janeiro' : label == 'Fev' ? 'Fevereiro' : label == 'Mar' ? 'Março' : label == 'Abr' ? 'Abril' : label == 'Mai' ? 'Maio' : label == 'Jun' ? 'Junho' : label == 'Jul' ? 'Julho' : label == 'Ago' ? 'Agosto' : label == 'Set' ? 'Setembro' : label == 'Out' ? 'Outubro' : label == 'Nov' ? 'Novembro' : 'Dezembro'}`}</p>
                <p className="font-bold text-dashboard-tea">
                    {`R$ ${payload[0].value.toFixed(2).replace('.', ',')}`}
                </p>
            </div>
        );
    }

    return null;
};

export default function BillingChart({ title, data }) {
    return (
        <div className="bg-background-card-dashboard rounded-[20px] p-5 shadow-md w-[1050px] h-[370px]">
            <div className="flex flex-col">
                <p className="text-xs font-bold mb-4 text-card-dashboard-capt-text">
                    {title}
                </p>
                <ResponsiveContainer width="100%" height={314}>
                    <AreaChart data={data}>
                        <defs>
                            <linearGradient
                                id="colorFat"
                                x1="0"
                                y1="0"
                                x2="0"
                                y2="1"
                            >
                                <stop
                                    offset="0%"
                                    stopColor="#319795"
                                    stopOpacity={0.99}
                                />
                                <stop
                                    offset="100%"
                                    stopColor="#319795"
                                    stopOpacity={0.05}
                                />
                            </linearGradient>
                        </defs>
                        <CartesianGrid
                            strokeDasharray="3 3"
                            stroke="#9ea3ae"
                            vertical={false}
                        />
                        <XAxis
                            dataKey="name"
                            tick={{ fill: '#9ea3ae', fontSize: 12 }}
                        />
                        <YAxis
                            dataKey="valorTotal"
                            tick={{ fill: '#9ea3ae', fontSize: 12 }}
                            tickFormatter={(v) => `${v}`}
                        />
                        <Tooltip content={<CustomTooltip />} />
                        <Area
                            type="monotone"
                            dataKey="valorTotal"
                            stroke="#0f766e"
                            fill="url(#colorFat)"
                            strokeWidth={2}
                        />
                    </AreaChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
}
