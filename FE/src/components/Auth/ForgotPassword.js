import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
// ĐÃ SỬA LỖI: Thay đổi cách import từ default sang named exports
import { sendOtpForResetPassword, resetPassword } from '../../services/authService'; 
import LoadingSpinner from '../Shared/LoadingSpinner';
import { Heart } from 'lucide-react'; // Import Heart icon
import '../../styles/Auth.css'; // Import CSS chung cho Auth

function ForgotPassword() {
  const [email, setEmail] = useState('');
  const [otpCode, setOtpCode] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmNewPassword, setConfirmNewPassword] = useState('');

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [step, setStep] = useState('sendOtp'); // 'sendOtp', 'verifyOtp', 'resetPassword'
  const [countdown, setCountdown] = useState(0); // Thời gian đếm ngược cho OTP

  const navigate = useNavigate();

  const handleSendOtp = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setMessage('');
    try {
      // SỬ DỤNG HÀM sendOtpForResetPassword TRỰC TIẾP
      const responseMessage = await sendOtpForResetPassword(email);
      setMessage(responseMessage + ' Vui lòng kiểm tra email.');
      setStep('verifyOtp');
      setCountdown(300); // Đặt thời gian đếm ngược 5 phút
    } catch (err) {
      setError(err.message || 'Gửi OTP thất bại. Vui lòng kiểm tra email.');
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyOtp = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setMessage('');
    try {
      // Backend của bạn xử lý verify OTP trong cùng endpoint resetPassword,
      // ở đây chúng ta chỉ chuyển bước mà không gọi API verify riêng.
      // Nếu Backend có endpoint verify riêng, bạn sẽ gọi nó ở đây.
      setMessage('Xác thực OTP thành công. Vui lòng đặt mật khẩu mới.');
      setStep('resetPassword');
    } catch (err) {
      setError(err.message || 'Xác thực OTP thất bại. Vui lòng kiểm tra mã OTP.');
    } finally {
      setLoading(false);
    }
  };

  const handleResetPassword = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setMessage('');

    if (newPassword.length < 6) {
      setError('Mật khẩu mới phải có ít nhất 6 ký tự.');
      setLoading(false);
      return;
    }
    if (newPassword !== confirmNewPassword) {
      setError('Mật khẩu xác nhận không khớp.');
      setLoading(false);
      return;
    }

    try {
      // SỬ DỤNG HÀM resetPassword TRỰC TIẾP
      const responseMessage = await resetPassword(email, otpCode, newPassword, confirmNewPassword);
      setMessage(responseMessage + ' Bạn sẽ được chuyển hướng đến trang đăng nhập.');
      setTimeout(() => {
        navigate('/login');
      }, 2000);
    } catch (err) {
      setError(err.message || 'Đặt lại mật khẩu thất bại. Vui lòng kiểm tra OTP và email.');
    } finally {
      setLoading(false);
    }
  };

  // Logic đếm ngược OTP
  React.useEffect(() => {
    let timer;
    if (step === 'verifyOtp' && countdown > 0) {
      timer = setInterval(() => {
        setCountdown(prev => prev - 1);
      }, 1000);
    } else if (countdown === 0 && step === 'verifyOtp') {
      setMessage('Mã OTP đã hết hạn. Vui lòng gửi lại.');
    }
    return () => clearInterval(timer);
  }, [step, countdown]);

  const formatTime = (seconds) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
  };


  return (
    <main>
      <div className="auth-container">
        {/* Logo và Slogan */}
        <div className="login-header-logo-section">
          <div className="login-logo-wrapper">
            <Heart className="login-logo-icon" fill="currentColor" />
          </div>
          <span className="login-logo-text">BloodLife</span>
          <p className="login-slogan-text">Đặt lại mật khẩu</p>
        </div>

        <div className="card shadow-lg auth-card">
          <div className="card-header bg-warning text-dark text-center auth-header">
            <h3>Đặt lại Mật khẩu</h3>
            <p className="form-subtitle">Theo các bước để đặt lại mật khẩu của bạn</p>
          </div>
          <div className="card-body p-4">
            {error && <div className="alert alert-danger">{error}</div>}
            {message && <div className="alert alert-info">{message}</div>}

            {step === 'sendOtp' && (
              <form onSubmit={handleSendOtp} className="auth-form-body forgot-password-form">
                <div className="mb-3">
                  <label htmlFor="email" className="form-label">Email</label>
                  <input
                    type="email"
                    className="form-control"
                    id="email"
                    name="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="Nhập địa chỉ email của bạn"
                    required
                    disabled={loading}
                  />
                </div>
                <button type="submit" className="btn btn-warning w-100" disabled={loading}>
                  {loading ? <LoadingSpinner /> : 'Gửi OTP để đặt lại'}
                </button>
              </form>
            )}

            {step === 'verifyOtp' && (
              <form onSubmit={handleVerifyOtp} className="auth-form-body forgot-password-form">
                <p className="text-center text-muted">Mã OTP đã được gửi đến <strong>{email}</strong>. Vui lòng nhập mã để tiếp tục.</p>
                <div className="mb-3">
                  <label htmlFor="otpCode" className="form-label">Mã OTP</label>
                  <input
                    type="text"
                    className="form-control"
                    id="otpCode"
                    name="otpCode"
                    value={otpCode}
                    onChange={(e) => setOtpCode(e.target.value)}
                    placeholder="Nhập mã OTP"
                    required
                  />
                </div>
                <div className="otp-countdown-wrapper">
                    <span className={`otp-countdown-text ${countdown === 0 ? 'expired' : ''}`}>
                      Mã OTP hết hạn sau: {formatTime(countdown)}
                    </span>
                    <button 
                      type="button" 
                      className="btn-link-resend-otp" 
                      onClick={handleSendOtp} 
                      disabled={loading || countdown > 0}
                    >
                      Gửi lại OTP
                    </button>
                </div>
                <button type="submit" className="btn btn-primary w-100" disabled={loading || countdown === 0}>
                  {loading ? <LoadingSpinner /> : 'Xác thực OTP'}
                </button>
              </form>
            )}

            {step === 'resetPassword' && (
              <form onSubmit={handleResetPassword} className="auth-form-body forgot-password-form">
                <div className="mb-3">
                  <label htmlFor="newPassword" className="form-label">Mật khẩu mới</label>
                  <input
                    type="password"
                    className="form-control"
                    id="newPassword"
                    name="newPassword"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    placeholder="Nhập mật khẩu mới"
                    required
                  />
                </div>
                <div className="mb-3">
                  <label htmlFor="confirmNewPassword" className="form-label">Xác nhận mật khẩu mới</label>
                  <input
                    type="password"
                    className="form-control"
                    id="confirmNewPassword"
                    name="confirmNewPassword"
                    value={confirmNewPassword}
                    onChange={(e) => setConfirmNewPassword(e.target.value)}
                    placeholder="Nhập lại mật khẩu mới"
                    required
                  />
                </div>
                <button type="submit" className="btn btn-success w-100" disabled={loading}>
                  {loading ? <LoadingSpinner /> : 'Đặt lại Mật khẩu'}
                </button>
              </form>
            )}

            <div className="mt-3 text-center">
              Nhớ mật khẩu? <Link to="/login">Đăng nhập</Link>
            </div>
          </div>
        </div>
      </div>
    </main>
  );
}

export default ForgotPassword;
