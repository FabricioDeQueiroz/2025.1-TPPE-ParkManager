import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export const showToast = ({
    message,
    type = 'default',
    navigateTo,
    duration = 3000,
    position = 'top-right',
    onClose,
}) => {
    toast(
        <div className="text-white font-semibold px-0.5 py-1">{message}</div>,
        {
            type,
            position,
            autoClose: duration,
            onClose: () => {
                if (navigateTo) navigateTo();
                if (onClose) onClose();
            },
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
            theme: 'colored',
        }
    );
};

export const ToastNotify = () => (
    <ToastContainer newestOnTop pauseOnFocusLoss={false} />
);
