import { FaRegCopyright } from 'react-icons/fa6';

const Footer = () => {
    return (
        <footer className="flex justify-start px-12 h-[100px] bg-color-footer w-full">
            <div className="flex items-center">
                <FaRegCopyright className="w-3.5 text-texto-footer" />
                <p className="text-texto-footer text-xs ml-3">
                    2025, Fabrício M. de Queiroz
                </p>
            </div>
        </footer>
    );
};

export default Footer;
