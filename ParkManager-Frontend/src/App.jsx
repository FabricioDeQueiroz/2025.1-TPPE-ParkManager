import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ToastNotify } from './components/Feedback/ToastNotify';
import PrivateRoute from './features/auth/PrivateRoute';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';

function App() {
    return (
        <>
            <BrowserRouter>
                <ToastNotify />
                <Routes>
                    <Route path="/parkmanager/" element={<Login />} />
                    <Route
                        path="/parkmanager/cadastro"
                        element={<Register />}
                    />
                    <Route
                        path="/parkmanager/dashboard"
                        element={
                            <PrivateRoute allowedTypes={[0, 1]}>
                                <Dashboard />
                            </PrivateRoute>
                        }
                    />
                </Routes>
            </BrowserRouter>
        </>
    );
}

export default App;
