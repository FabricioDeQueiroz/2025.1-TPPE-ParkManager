import Loading from '../Feedback/Loading';

const LoginRegisterButton = ({ label, type = 'submit', isLoading }) => {
    return (
        <button
            type={type}
            className={`hover:cursor-pointer rounded-lg text-sm text-button-text bg-button-color hover:bg-button-color-hover font-bold w-[100%] h-[50px] self-center`}
        >
            {isLoading ? <Loading /> : label}
        </button>
    );
};

export default LoginRegisterButton;
