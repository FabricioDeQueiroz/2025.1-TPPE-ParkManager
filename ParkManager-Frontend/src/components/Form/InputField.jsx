import { useState } from 'react';
import { FaEye, FaEyeSlash } from 'react-icons/fa';

const InputField = ({
    label,
    type = 'text',
    value,
    onChange,
    placeholder = '',
    required = false,
}) => {
    const [showPassword, setShowPassword] = useState(false);
    const isPassword = type === 'password';

    return (
        <div className="flex flex-col gap-1">
            <label className="text-sm font-semibold text-texto-login ml-1">
                {label}
            </label>
            <div className="relative">
                <input
                    type={isPassword && showPassword ? 'text' : type}
                    value={value}
                    onChange={onChange}
                    required={required}
                    placeholder={placeholder}
                    className="transition duration-300 hover:border-field-border-active hover:ring-1 h-12 w-full pr-10 rounded-lg border border-field-login bg-field-border-login px-4 py-2 text-sm text-texto-login placeholder-placeholder-login focus:outline-none focus:ring-1 focus:ring-field-border-active focus:bg-field-border-login focus:border-field-border-active"
                />
                {isPassword && (
                    <button
                        type="button"
                        onClick={() => setShowPassword((prev) => !prev)}
                        className="absolute right-5 top-1/2 -translate-y-1/2 text-texto-login focus:outline-none hover:cursor-pointer"
                        tabIndex={-1}
                    >
                        {showPassword ? (
                            <FaEyeSlash size={18} />
                        ) : (
                            <FaEye size={18} />
                        )}
                    </button>
                )}
            </div>
        </div>
    );
};

export default InputField;
