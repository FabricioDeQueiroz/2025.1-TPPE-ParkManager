import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './theme/theme.css';
import App from './App.jsx';
import { AuthProvider } from './features/auth/AuthContext.jsx';
import Footer from './components/NavBar/Footer.jsx';

createRoot(document.getElementById('root')).render(
    <StrictMode>
        <AuthProvider>
            {/* <div className="min-h-screen flex flex-col justify-between"> */}
            <App />
            <Footer />
            {/* </div> */}
        </AuthProvider>
    </StrictMode>
);
