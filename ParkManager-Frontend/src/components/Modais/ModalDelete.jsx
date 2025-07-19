const ModalDelete = ({ type, action }) => {
    return (
        <dialog id="delete-modal" className="modal">
            <div className="bg-background-card-dashboard modal-box w-[546px] min-w-[546px] h-[252px] rounded-[6px] shadow-lg relative">
                <form method="dialog">
                    <button className="btn btn-sm btn-circle btn-ghost text-card-dashboard-text absolute right-2 top-2 text-xl">
                        ✕
                    </button>
                </form>
                <h3 className="text-lg text-card-dashboard-text">
                    {`Tem certeza que deseja deletar esse ${type}?`}
                </h3>
                <p className="text-lg py-5 font-bold text-card-dashboard-text mb-5">
                    Essa ação é irreversível!
                </p>
                <button
                    onClick={action}
                    className="w-full h-[80px] font-bold text-[32px] bg-dashboard-red-500 hover:opacity-80 text-dashboard-create-button-text rounded-lg"
                >
                    DELETAR
                </button>
            </div>
        </dialog>
    );
};

export default ModalDelete;
