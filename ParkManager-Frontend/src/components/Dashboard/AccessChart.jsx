import {
    BarChart,
    Bar,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
} from 'recharts';

const data = [
    { name: 'Jul', 'Quantidade de Acessos': 1300 },
    { name: 'Aug', 'Quantidade de Acessos': 1000 },
    { name: 'Sep', 'Quantidade de Acessos': 2000 },
    { name: 'Oct', 'Quantidade de Acessos': 1200 },
    { name: 'Nov', 'Quantidade de Acessos': 900 },
    { name: 'Dec', 'Quantidade de Acessos': 1900 },
];

const CustomTooltip = ({ active, payload, label }) => {
    if (active && payload && payload.length) {
        return (
            <div className="p-4 bg-background-card-dashboard border border-card-dashboard-capt-text rounded-md shadow-lg">
                <p className="font-bold text-card-dashboard-text">{`Mês: ${label}`}</p>
                <p className="font-bold text-dashboard-tea">
                    {`Acessos: ${payload[0].value}`}
                </p>
            </div>
        );
    }

    return null;
};

export default function AccessChart() {
    return (
        <div className="bg-background-card-dashboard rounded-[20px] p-5 shadow-md min-w-[510px] h-[490px]">
            <div className="flex flex-col">
                <p className="text-xs font-bold mb-14 text-card-dashboard-capt-text">
                    QUANTIDADE DE ACESSOS
                </p>
                <ResponsiveContainer width="100%" height={394}>
                    <BarChart data={data}>
                        <CartesianGrid
                            strokeDasharray="3 3"
                            stroke="#6b7280"
                            vertical={false}
                        />
                        <XAxis
                            dataKey="name"
                            tick={{ fill: '#6b7280', fontSize: 12 }}
                        />
                        <YAxis tick={{ fill: '#6b7280', fontSize: 12 }} />
                        <Tooltip
                            content={<CustomTooltip />}
                            cursor={{ fill: 'rgba(200, 200, 200, 0.2)' }}
                        />
                        <Bar
                            dataKey="Quantidade de Acessos"
                            fill="#319795"
                            radius={[4, 4, 0, 0]}
                            barSize={12}
                        />
                    </BarChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
}
