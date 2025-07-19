import { FaMoon, FaSun } from 'react-icons/fa';
import { useEffect, useState } from 'react';

export const ThemeToggleButton = () => {
    const [isDark, setIsDark] = useState(false);

    useEffect(() => {
        const storedTheme = localStorage.getItem('theme');
        const prefersDark = storedTheme === 'dark';

        setIsDark(prefersDark);
        const html = document.documentElement;
        prefersDark
            ? html.classList.add('dark')
            : html.classList.remove('dark');
    }, []);

    const toggleTheme = () => {
        const html = document.documentElement;
        const newIsDark = !isDark;

        setIsDark(newIsDark);
        localStorage.setItem('theme', newIsDark ? 'dark' : 'light');

        if (newIsDark) {
            html.classList.add('dark');
        } else {
            html.classList.remove('dark');
        }
    };

    return (
        <div
            onClick={toggleTheme}
            className="group flex flex-row items-center bg-background-option-de rounded-lg py-3 px-4 mb-4 mx-3 hover:bg-background-option-button-ac cursor-pointer border-2 border-background-option-button-de"
        >
            <div className="bg-background-option-button-de w-9 h-9 rounded-lg mr-3 flex items-center justify-center group-hover:bg-background-option-icon-de">
                {isDark ? (
                    <FaSun className="text-background-option-icon-de w-6 h-6 group-hover:text-theme-toggle-button" />
                ) : (
                    <FaMoon className="text-background-option-icon-de w-6 h-6 group-hover:text-theme-toggle-button" />
                )}
            </div>
            <p className="text-text-option-de font-bold group-hover:text-background-option-icon-ac">
                {isDark ? 'Modo Claro' : 'Modo Escuro'}
            </p>
        </div>
    );
};
