import {
    AreaChart,
    Area,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
} from 'recharts';

const data = [
    { name: 'Jan', value: 19000 },
    { name: 'Feb', value: 21000 },
    { name: 'Mar', value: 22000 },
    { name: 'Apr', value: 32000 },
    { name: 'May', value: 35000 },
    { name: 'Jun', value: 47000 },
    { name: 'Jul', value: 40000 },
    { name: 'Aug', value: 30000 },
    { name: 'Sep', value: 33000 },
    { name: 'Oct', value: 21000 },
    { name: 'Nov', value: 39000 },
    { name: 'Dec', value: 43000 },
];

const CustomTooltip = ({ active, payload, label }) => {
    if (active && payload && payload.length) {
        return (
            <div className="p-4 bg-background-card-dashboard border border-card-dashboard-capt-text rounded-md shadow-lg">
                <p className="font-bold text-card-dashboard-text">{`Mês: ${label}`}</p>
                <p className="font-bold text-dashboard-tea">
                    {`R$ ${payload[0].value}`}
                </p>
            </div>
        );
    }

    return null;
};

export default function BillingChart() {
    return (
        <div className="bg-background-card-dashboard rounded-[20px] p-5 shadow-md w-[1050px] h-[490px]">
            <div className="flex flex-col">
                <p className="text-xs font-bold mb-14 text-card-dashboard-capt-text">
                    FATURAMENTO NO DECORRER DOS MESES
                </p>
                <ResponsiveContainer width="100%" height={394}>
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
                            stroke="#6b7280"
                            vertical={false}
                        />
                        <XAxis
                            dataKey="name"
                            tick={{ fill: '#6b7280', fontSize: 12 }}
                        />
                        <YAxis
                            tick={{ fill: '#6b7280', fontSize: 12 }}
                            tickFormatter={(v) => `${v}`}
                        />
                        <Tooltip content={<CustomTooltip />} />
                        <Area
                            type="monotone"
                            dataKey="value"
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
