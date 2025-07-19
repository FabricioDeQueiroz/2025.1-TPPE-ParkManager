const ActionButton = ({ icon: Icon, label, color, action }) => {
    return (
        <div
            onClick={action}
            className={`h-[31px] w-fit hover:cursor-pointer flex flex-row justify-between items-center gap-2 px-2.5 text-sm font-bold bg-card-dashboard-line-table rounded-lg hover:opacity-70 ${color}`}
        >
            {Icon && <Icon className={`w-4 h-4 ${color}`} />}
            {label}
        </div>
    );
};

export default ActionButton;
