import React, { useState, useEffect, useContext } from 'react';
import { AuthContext } from '../context/AuthContext';
import * as dashboardService from '../services/dashboardService';
import * as userService from '../services/userService'; // Để lấy danh sách user nếu cần
import Header from '../components/Header';
import Navbar from '../components/Navbar';
import Footer from '../components/Footer';
import './Dashboard.css'; // Tạo file CSS này

function DashboardPage() {
    const { isAuthenticated, userRole } = useContext(AuthContext);
    const [summary, setSummary] = useState(null);
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchData = async () => {
            if (!isAuthenticated || userRole !== 'Admin') {
                setError('Bạn không có quyền truy cập trang này.');
                setLoading(false);
                return;
            }
            try {
                const dashboardSummary = await dashboardService.getDashboardSummary();
                setSummary(dashboardSummary);

                const allUsers = await userService.getAllUsers();
                setUsers(allUsers);

            } catch (err) {
                setError(err.message || 'Không thể tải dữ liệu dashboard.');
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, [isAuthenticated, userRole]);

    if (loading) {
        return <p>Đang tải dữ liệu...</p>;
    }

    if (error) {
        return <p className="error-message">{error}</p>;
    }

    return (
        <div className="dashboard-container">
            <Header />
            <Navbar />
            <main className="dashboard-content">
                <h2>Dashboard Quản trị</h2>
                {summary && (
                    <div className="summary-cards">
                        <div className="card">
                            <h3>Tổng số người dùng</h3>
                            <p>{summary.totalUsers}</p>
                        </div>
                        <div className="card">
                            <h3>Tổng số đơn vị máu</h3>
                            <p>{summary.totalBloodUnits}</p>
                        </div>
                        {/* Thêm các thống kê khác từ summary DTO của bạn */}
                    </div>
                )}

                <h3>Danh sách người dùng</h3>
                <table className="users-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Tên người dùng</th>
                            <th>Email</th>
                            <th>Vai trò</th>
                            <th>Trạng thái</th>
                            <th>Hành động</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map(user => (
                            <tr key={user.userId}>
                                <td>{user.userId}</td>
                                <td>{user.username}</td>
                                <td>{user.email}</td>
                                <td>{user.roleId}</td> {/* Hiển thị RoleId, có thể fetch tên Role sau */}
                                <td>{user.isActive ? 'Active' : 'Inactive'}</td>
                                <td>
                                    <button onClick={() => console.log('Edit user', user.userId)}>Sửa</button>
                                    <button onClick={() => console.log('Delete user', user.userId)}>Xóa</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </main>
            <Footer />
        </div>
    );
}

export default DashboardPage;