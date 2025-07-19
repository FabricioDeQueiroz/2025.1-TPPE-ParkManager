import { FaCheckCircle } from 'react-icons/fa';
import gerente from '../../assets/images/graph.svg';
import cliente from '../../assets/images/car.svg';

const options = [
    {
        id: 0,
        label: 'Gerente',
        image: gerente,
    },
    {
        id: 1,
        label: 'Cliente',
        image: cliente,
    },
];

export default function ImageInputRadio({ value, onChange }) {
    return (
        <div className="flex w-full self-center justify-center gap-x-10 mb-7">
            {options.map((option) => {
                const selected = value === option.id;
                return (
                    <label
                        key={option.id}
                        className={`group relative cursor-pointer p-4 rounded-lg border-2 w-36 flex flex-col items-center justify-center ${
                            selected
                                ? 'border-field-border-active'
                                : 'border-field-login hover:border-button-color-hover'
                        }`}
                    >
                        <input
                            type="radio"
                            name="role"
                            className="sr-only"
                            checked={selected}
                            required={option.id === 0}
                            onChange={() => onChange(option.id)}
                        />
                        <img
                            src={option.image}
                            alt={option.label}
                            className="h-20 object-contain mb-2"
                        />
                        <span
                            className={`text-sm ${
                                selected
                                    ? 'text-texto-login font-bold'
                                    : 'text-placeholder-login group-hover:text-button-color-hover group-hover:font-bold'
                            }`}
                        >
                            {option.label}
                        </span>

                        {selected && (
                            <div className="absolute bottom-0 right-0 pl-2 pt-2 transform translate-x-1/2 translate-y-1/2 text-field-border-active rounded-full p-1">
                                <FaCheckCircle size={22} />
                            </div>
                        )}
                    </label>
                );
            })}
        </div>
    );
}
