import { useAuth } from '../features/auth/AuthContext';
import Sidebar from '../components/Sidebar/Sidebar';
import DashboardCustomer from './DashboardCustomer';
import DashboardManager from './DashboardManager';

const Dashboard = () => {
    const { userType } = useAuth();

    return (
        <div className="flex flex-row">
            <Sidebar />
            {userType === 0 ? <DashboardManager /> : <DashboardCustomer />}
        </div>
    );
};

export default Dashboard;
