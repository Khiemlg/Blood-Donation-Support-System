import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

// Import các trang chính của ứng dụng
import HomePage from './pages/Home/HomePage'; // Đảm bảo HomePage nằm trong src/pages/Home/
import LoginPage from './components/Auth/Login'; // Đảm bảo Login.js nằm trong src/components/Auth/
import RegisterPage from './components/Auth/Register'; // Đảm bảo Register.js nằm trong src/components/Auth/
import ForgotPassword from './components/Auth/ForgotPassword'; // Đảm bảo ForgotPassword.js nằm trong src/components/Auth/

// Import các trang placeholder
import BlogPage from './pages/BlogPage'; // Cần tạo file này
import DocumentationPage from './pages/DocumentationPage'; // Cần tạo file này
import NotFoundPage from './pages/NotFoundPage'; // Cần tạo file này

// Các trang hoặc components khác (hiện đang comment out, bật khi cần)
// import DashboardPage from './components/Home/Dashboard'; // Trang sau khi đăng nhập thành công
// import UserProfile from './components/User/UserProfile'; // Trang hồ sơ người dùng
// import ManageUsers from './components/Admin/ManageUsers'; // Trang quản lý người dùng (Admin)
// import ManageRoles from './components/Admin/ManageRoles'; // Trang quản lý vai trò (Admin)


// Import AuthContext và PrivateRoute
import { AuthProvider } from './context/AuthContext'; // Đảm bảo AuthContext.js nằm trong src/context/
import PrivateRoute from './components/Router/PrivateRoute'; // Đảm bảo PrivateRoute.js nằm trong src/components/Shared/

// Import CSS chung
import './styles/App.css'; // Đảm bảo App.css nằm trong src/styles/

function App() {
    return (
        <Router>
            {/* Bọc toàn bộ ứng dụng bằng AuthProvider để quản lý trạng thái đăng nhập */}
            <AuthProvider> 
                <div className="App">
                    <Routes>
                        {/* Public Routes (Không yêu cầu đăng nhập) */}
                        <Route path="/" element={<HomePage />} />
                        <Route path="/login" element={<LoginPage />} />
                        <Route path="/register" element={<RegisterPage />} />
                        <Route path="/forgot-password" element={<ForgotPassword />} />
                        <Route path="/blog" element={<BlogPage />} />
                        <Route path="/documentation" element={<DocumentationPage />} />

                        {/* Private Routes (Yêu cầu đăng nhập) */}
                        <Route element={<PrivateRoute />}>
                            {/* <Route path="/dashboard" element={<DashboardPage />} /> */}
                            {/* <Route path="/profile" element={<UserProfile />} /> */}
                            {/* Thêm các PrivateRoute khác cho người dùng thông thường (Member, Staff) */}
                        </Route>

                        {/* Admin Routes (Yêu cầu đăng nhập và có vai trò Admin) */}
                        <Route element={<PrivateRoute roles={['Admin']} />}>
                            {/* <Route path="/admin/users" element={<ManageUsers />} /> */}
                            {/* <Route path="/admin/roles" element={<ManageRoles />} /> */}
                            {/* Thêm các PrivateRoute khác cho Admin */}
                        </Route>
                        
                        {/* Route cho trường hợp không có quyền truy cập */}
                        <Route path="/unauthorized" element={<h2>Bạn không có quyền truy cập trang này.</h2>} />

                        {/* Fallback Route (Xử lý các URL không khớp - trang 404) */}
                        <Route path="*" element={<NotFoundPage />} />
                    </Routes>
                </div>
            </AuthProvider>
        </Router>
    );
}

export default App;
