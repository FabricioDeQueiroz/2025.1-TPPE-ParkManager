import {
    BarChart,
    Bar,
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
                    {`Acessos: ${payload[0].value}`}
                </p>
            </div>
        );
    }

    return null;
};

export default function AccessChart({ title, data }) {
    return (
        <div className="bg-background-card-dashboard rounded-[20px] p-5 shadow-md min-w-[510px] h-[370px]">
            <div className="flex flex-col">
                <p className="text-xs font-bold mb-4 text-card-dashboard-capt-text">
                    {title}
                </p>
                <ResponsiveContainer width="100%" height={314}>
                    <BarChart data={data}>
                        <CartesianGrid
                            strokeDasharray="3 3"
                            stroke="#9ea3ae"
                            vertical={false}
                        />
                        <XAxis
                            dataKey="name"
                            tick={{ fill: '#9ea3ae', fontSize: 10 }}
                        />
                        <YAxis tick={{ fill: '#9ea3ae', fontSize: 12 }} />
                        <Tooltip
                            content={<CustomTooltip />}
                            cursor={{ fill: 'rgba(200, 200, 200, 0.2)' }}
                        />
                        <Bar
                            dataKey="quantidade"
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
