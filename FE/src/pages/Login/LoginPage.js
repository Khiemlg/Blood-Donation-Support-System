import React, { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../../context/AuthContext';
import * as authService from '../../services/authService'; // Sẽ tạo sau
import '../styles/Auth.css'; // CSS cho đăng nhập/đăng ký

function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();
    const { login } = useContext(AuthContext); // Lấy hàm login từ AuthContext

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        try {
            const token = await authService.login(email, password);
            login(token); // Lưu token và cập nhật trạng thái xác thực
            navigate('/dashboard'); // Chuyển hướng đến dashboard hoặc trang nào đó sau khi đăng nhập
        } catch (err) {
            setError(err.message || 'Đăng nhập thất bại. Vui lòng kiểm tra lại email và mật khẩu.');
        }
    };

    return (
        <div className="auth-container">
            <h2>Đăng nhập</h2>
            <form onSubmit={handleSubmit} className="auth-form">
                {error && <p className="error-message">{error}</p>}
                <div className="form-group">
                    <label htmlFor="email">Email:</label>
                    <input
                        type="email"
                        id="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="password">Mật khẩu:</label>
                    <input
                        type="password"
                        id="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit" className="submit-button">Đăng nhập</button>
                <p className="switch-auth">
                    Chưa có tài khoản? <Link to="/register">Đăng ký ngay</Link>
                </p>
                <p className="forgot-password">
                    <Link to="/forgot-password">Quên mật khẩu?</Link>
                </p>
            </form>
        </div>
    );
}

export default LoginPage;