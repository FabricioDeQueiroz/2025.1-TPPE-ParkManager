import { useLocation, useNavigate } from 'react-router-dom';

export const SidebarButton = ({ icon: Icon, label, to }) => {
    const location = useLocation();
    const navigate = useNavigate();

    const isActive = location.pathname === to;

    const handleClick = () => {
        if (!isActive) {
            navigate(to);
        }
    };

    return (
        <div
            onClick={handleClick}
            className={`flex flex-row items-center rounded-lg py-3 px-4 mb-4 mx-3
        ${
            isActive
                ? 'bg-background-option-ac shadow-lg'
                : 'bg-background-option-de hover:bg-background-option-button-ac cursor-pointer group border-2 border-background-option-button-de'
        }
      `}
        >
            <div
                className={`
          w-9 h-9 rounded-lg mr-3 flex items-center justify-center
          ${
              isActive
                  ? 'bg-background-option-button-ac'
                  : 'bg-background-option-button-de group-hover:bg-background-option-icon-de'
          }
        `}
            >
                {Icon && (
                    <Icon
                        className={`
            w-6 h-6
            ${
                isActive
                    ? 'text-background-option-icon-ac'
                    : 'text-background-option-icon-de group-hover:text-background-option-icon-ac'
            }
          `}
                    />
                )}
            </div>
            <p
                className={`
          font-bold
          ${
              isActive
                  ? 'text-text-option-ac'
                  : 'text-text-option-de group-hover:text-background-option-icon-ac'
          }
        `}
            >
                {label}
            </p>
        </div>
    );
};
