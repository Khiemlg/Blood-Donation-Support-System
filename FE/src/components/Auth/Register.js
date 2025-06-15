import React, { useState, useEffect, useContext } from 'react';
import { useNavigate, Link } from 'react-router-dom';
// Đã import các hàm cụ thể từ authService để tránh lỗi default export
import { register, sendOtpForRegistration } from '../../services/authService'; 
import LoadingSpinner from '../Shared/LoadingSpinner';
import { Heart } from 'lucide-react';
import '../../styles/Auth.css'; // Import CSS chung cho Auth
import '../../styles/Register.css'; // Import CSS riêng cho Register
import { AuthContext } from '../../context/AuthContext'; // Import AuthContext để tự động đăng nhập sau khi đăng ký

function Register() {
  // State chứa tất cả dữ liệu form đăng ký
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    otpCode: '', // OTP Code sẽ được nhập vào đây
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [otpSent, setOtpSent] = useState(false); // Trạng thái để kiểm soát hiển thị ô OTP và nút
  const [countdown, setCountdown] = useState(0); // Thời gian đếm ngược cho OTP

  const navigate = useNavigate();
  const { login } = useContext(AuthContext); // Lấy hàm login từ AuthContext

  /**
   * Xử lý thay đổi các trường input trong form.
   * @param {Object} e - Đối tượng sự kiện.
   */
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  /**
   * Xử lý yêu cầu gửi OTP.
   * Kiểm tra các trường username, email, password, confirmPassword trước khi gửi OTP.
   * @param {Object} e - Đối tượng sự kiện.
   */
  const handleSendOtpRequest = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setMessage('');

    // Basic client-side validation before sending OTP
    if (!formData.username || !formData.email || !formData.password || !formData.confirmPassword) {
      setError('Vui lòng điền đầy đủ tất cả các trường.');
      setLoading(false);
      return;
    }

    if (formData.password.length < 6) {
        setError('Mật khẩu phải có ít nhất 6 ký tự.');
        setLoading(false);
        return;
    }

    if (formData.password !== formData.confirmPassword) {
      setError('Mật khẩu và xác nhận mật khẩu không khớp.');
      setLoading(false);
      return;
    }

    try {
      // Gọi API để gửi OTP (Backend: SendOtpAsync)
      const responseMessage = await sendOtpForRegistration(formData.email); 
      setMessage(responseMessage + ' Vui lòng kiểm tra email của bạn để nhận mã OTP. Mã OTP sẽ hết hạn trong 5 phút.');
      setOtpSent(true); // Đánh dấu là OTP đã được gửi
      setCountdown(300); // Đặt thời gian đếm ngược 5 phút (300 giây)
    } catch (err) {
      setError(err.message || 'Gửi OTP thất bại. Email có thể đã tồn tại hoặc lỗi hệ thống.');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Xử lý việc hoàn tất đăng ký với mã OTP.
   * @param {Object} e - Đối tượng sự kiện.
   */
  const handleCompleteRegistration = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setMessage('');

    if (!formData.otpCode) {
      setError('Vui lòng nhập mã OTP.');
      setLoading(false);
      return;
    }
    
    try {
      // Gọi API đăng ký cuối cùng (Backend: RegisterAsync)
      // ĐÃ SỬA: Loại bỏ việc gán giá trị trả về cho biến 'token' vì nó không được sử dụng
      await register(
        formData.username,
        formData.email,
        formData.password,
        formData.confirmPassword,
        formData.otpCode
      ); 
      
      // Nếu đăng ký thành công, tự động đăng nhập người dùng thông qua AuthContext
      await login(formData.email, formData.password);

      setMessage('Đăng ký tài khoản thành công! Bạn sẽ được chuyển hướng về trang chủ.');
      setTimeout(() => {
        navigate('/login'); // Điều hướng về trang chủ hoặc dashboard
      }, 2000);

    } catch (err) {
      setError(err.message || 'Đăng ký thất bại. Vui lòng kiểm tra thông tin và mã OTP.');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Hook useEffect để quản lý đồng hồ đếm ngược OTP.
   */
  useEffect(() => {
    let timer;
    if (otpSent && countdown > 0) {
      timer = setInterval(() => {
        setCountdown(prev => prev - 1);
      }, 1000);
    } else if (countdown === 0 && otpSent) {
      setMessage('Mã OTP đã hết hạn. Vui lòng gửi lại OTP mới.');
    }
    return () => clearInterval(timer);
  }, [otpSent, countdown]);

  /**
   * Định dạng thời gian từ giây sang MM:SS.
   * @param {number} seconds - Thời gian tính bằng giây.
   * @returns {string} Chuỗi thời gian đã định dạng.
   */
  const formatTime = (seconds) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
  };

  return (
    <main>
      <div className="auth-container">
        {/* Logo và Slogan Section */}
        <div className="login-header-logo-section">
          <div className="login-logo-wrapper">
            <Heart className="login-logo-icon" fill="currentColor" />
          </div>
          <span className="login-logo-text">BloodLife</span>
          <p className="login-slogan-text">Đăng ký tài khoản mới</p>
        </div>

        <div className="card shadow-lg auth-card">
          <div className="card-header bg-success text-white text-center auth-header">
            <h3>Đăng ký tài khoản</h3>
            <p className="form-subtitle">Tạo tài khoản mới của bạn để tham gia cùng chúng tôi</p>
          </div>
          <div className="card-body p-4">
            {error && <div className="alert alert-danger">{error}</div>}
            {message && <div className="alert alert-info">{message}</div>}
            
            <form onSubmit={otpSent ? handleCompleteRegistration : handleSendOtpRequest} className="auth-form-body register-form">
              {/* Các trường thông tin cơ bản, luôn hiển thị */}
              <div className="mb-3">
                <label htmlFor="username" className="form-label">Tên đăng nhập (Username)</label>
                <input
                  type="text"
                  className="form-control"
                  id="username"
                  name="username"
                  value={formData.username}
                  onChange={handleChange}
                  placeholder="Tên đăng nhập của bạn"
                  required
                  disabled={otpSent || loading} /* Vô hiệu hóa sau khi OTP đã gửi */
                />
              </div>
              <div className="mb-3">
                <label htmlFor="email" className="form-label">Email</label>
                <input
                  type="email"
                  className="form-control"
                  id="email"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  placeholder="Nhập địa chỉ email"
                  required
                  disabled={otpSent || loading} /* Vô hiệu hóa sau khi OTP đã gửi */
                />
              </div>
              <div className="mb-3">
                <label htmlFor="password" className="form-label">Mật khẩu</label>
                <input
                  type="password"
                  className="form-control"
                  id="password"
                  name="password"
                  value={formData.password}
                  onChange={handleChange}
                  placeholder="Tạo mật khẩu mạnh"
                  required
                  disabled={otpSent || loading} /* Vô hiệu hóa sau khi OTP đã gửi */
                />
              </div>
              <div className="mb-3">
                <label htmlFor="confirmPassword" className="form-label">Xác nhận mật khẩu</label>
                <input
                  type="password"
                  className="form-control"
                  id="confirmPassword"
                  name="confirmPassword"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  placeholder="Nhập lại mật khẩu"
                  required
                  disabled={otpSent || loading} /* Vô hiệu hóa sau khi OTP đã gửi */
                />
              </div>
              
              {/* Nút "Gửi OTP" hoặc ô nhập OTP và nút "Đăng ký" */}
              {!otpSent ? (
                <button type="submit" className="btn btn-primary w-100" disabled={loading}>
                  {loading ? <LoadingSpinner /> : 'Gửi OTP để đăng ký'}
                </button>
              ) : (
                <>
                  <div className="mb-3">
                    <label htmlFor="otpCode" className="form-label">Mã OTP</label>
                    <input
                      type="text"
                      className="form-control"
                      id="otpCode"
                      name="otpCode"
                      value={formData.otpCode}
                      onChange={handleChange}
                      placeholder="Nhập mã OTP"
                      required
                      disabled={loading || countdown === 0}
                    />
                  </div>
                  <div className="otp-countdown-wrapper">
                      <span className={countdown === 0 ? "otp-countdown-text expired" : "otp-countdown-text"}> 
                        Mã OTP hết hạn sau: {formatTime(countdown)}
                      </span>
                      <button 
                        type="button" 
                        className="btn-link-resend-otp" 
                        onClick={handleSendOtpRequest} // Quay lại bước gửi OTP ban đầu
                        disabled={loading || countdown > 0}
                      >
                        Gửi lại OTP
                      </button>
                    </div>
                  <button type="submit" className="btn btn-success w-100" disabled={loading || countdown === 0}>
                    {loading ? <LoadingSpinner /> : 'Hoàn tất Đăng ký'}
                  </button>
                </>
              )}
            </form>

            <div className="mt-3 text-center">
              Đã có tài khoản? <Link to="/login">Đăng nhập</Link>
            </div>
          </div>
        </div>
      </div>
    </main>
  );
}

export default Register;
