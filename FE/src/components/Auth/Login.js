import React, { useState, useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthContext } from '../../context/AuthContext'; // Import AuthContext
import LoadingSpinner from '../Shared/LoadingSpinner';
import '../../styles/Auth.css';

function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { login } = useContext(AuthContext); // Sử dụng hàm login từ AuthContext

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      // Gọi hàm login từ AuthContext.
      // Hàm login trong AuthContext đã được cập nhật để trả về response.user
      // giúp bạn lấy role để điều hướng nếu cần.
      const response = await login(email, password); 
      
      // Kiểm tra vai trò của người dùng và điều hướng tương ứng.
      // Đảm bảo rằng `response.user.role` có giá trị và khớp với các vai trò của bạn.
      if (response.user && response.user.role === 'Admin') { 
        navigate('/admin/dashboard'); 
      } else if (response.user && response.user.role === 'Staff') { 
        navigate('/staff/dashboard'); 
      } else {
        navigate('/'); // Điều hướng về trang chủ mặc định hoặc dashboard cho Member
      }

    } catch (err) {
      setError(err.message || 'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <main>
      <div className="auth-container">
        <div className="card shadow-lg auth-card">
          <div className="card-header bg-primary text-white text-center auth-header">
            <h3>Đăng nhập</h3>
          </div>
          <div className="card-body p-4">
            {error && <div className="alert alert-danger">{error}</div>}
            <form onSubmit={handleSubmit} className="auth-form-body">
              <div className="mb-3">
                <label htmlFor="email" className="form-label">Email</label>
                <input
                  type="email"
                  className="form-control"
                  id="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="password" className="form-label">Mật khẩu</label>
                <input
                  type="password"
                  className="form-control"
                  id="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </div>
              <button type="submit" className="btn btn-primary w-100" disabled={loading}>
                {loading ? <LoadingSpinner /> : 'Đăng nhập'}
              </button>
            </form>
            <div className="mt-3 text-center">
              <Link to="/forgot-password">Quên mật khẩu?</Link>
            </div>
            <div className="mt-2 text-center">
              Chưa có tài khoản? <Link to="/register">Đăng ký ngay</Link>
            </div>
          </div>
        </div>
      </div>
    </main>
  );
}

export default Login;
