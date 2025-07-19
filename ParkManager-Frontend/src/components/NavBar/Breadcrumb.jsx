import { useNavigate } from 'react-router-dom';
import { MdOutlineArrowForwardIos } from 'react-icons/md';

const Breadcrumb = ({ lista, caminhos = [] }) => {
    const navigate = useNavigate();

    return (
        <div className="flex items-center text-sm text-text-breadcrumb font-medium space-x-1">
            {lista.map((item, index) => {
                const isLast = index === lista.length - 1;
                const path = caminhos[index];

                return (
                    <div key={index} className="flex items-center">
                        {!isLast && path ? (
                            <button
                                onClick={() => navigate(path)}
                                className="hover:underline transition-colors hover:cursor-pointer"
                            >
                                {item}
                            </button>
                        ) : (
                            <span className="text-text-breadcrumb font-semibold">
                                {item}
                            </span>
                        )}
                        {index < lista.length - 1 && (
                            <MdOutlineArrowForwardIos className="text-text-breadcrumb w-3.5 h-3.5 mx-2" />
                        )}
                    </div>
                );
            })}
        </div>
    );
};

export default Breadcrumb;
