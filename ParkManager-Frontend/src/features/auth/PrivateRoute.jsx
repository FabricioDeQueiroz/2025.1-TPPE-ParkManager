import { useAuth } from './AuthContext';
import { Navigate } from 'react-router-dom';

const PrivateRoute = ({ children, allowedTypes }) => {
    const { token, user, userType } = useAuth();

    if (!token || !user) {
        return <Navigate to="/" replace />;
    }

    if (allowedTypes && !allowedTypes.includes(userType)) {
        return <Navigate to="/dashboard" replace />;
    }

    return children;
};

export default PrivateRoute;
