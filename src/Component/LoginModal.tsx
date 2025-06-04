// src/Component/LoginModal.tsx
import React, { useState } from 'react';
import axios from 'axios';
import { useAuth } from '../Context/AuthContext.tsx';
import { loginUser } from '../service/serviceauth.tsx';
import { useNavigate } from 'react-router-dom';

interface LoginModalProps {
  onClose: () => void; // Hàm để đóng modal
  onLoginSuccess: () => void; // Hàm gọi khi đăng nhập thành công
}

const LoginModal: React.FC<LoginModalProps> = ({ onClose, onLoginSuccess }) => {
  const [email, setEmail] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  const [message, setMessage] = useState<string>('');
  const [isSuccess, setIsSuccess] = useState<boolean | null>(null);

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setEmail(e.target.value);
  };

  const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPassword(e.target.value);
  };

  const handleLogin = async () => {
    setMessage('');
    setIsSuccess(null);

    if (!email || !password) {
      setMessage('Vui lòng nhập đầy đủ email và mật khẩu.');
      setIsSuccess(false);
      return;
    }

    try {
      const response = await loginUser(email, password); // Gọi API đăng nhập

      // Kiểm tra cấu trúc phản hồi từ backend. Backend phải trả về `data.data.token` và `data.data.user`
      if (response.data && response.data.data && response.data.data.token && response.data.data.user) {
        const { token, user } = response.data.data;
        login(token, user); // Lưu token và user vào AuthContext

        setMessage('Đăng nhập thành công!');
        setIsSuccess(true);

        setEmail('');
        setPassword('');

        // Đợi một chút để người dùng thấy thông báo thành công, sau đó đóng modal
        setTimeout(() => {
          onLoginSuccess(); // Thông báo cho HomePage rằng đăng nhập thành công
          onClose(); // Đóng modal
        }, 500);

      } else {
        setMessage('Phản hồi từ server không hợp lệ. Vui lòng thử lại.');
        setIsSuccess(false);
        console.error('Phản hồi API không đúng định dạng:', response.data); // Log để debug
      }

    } catch (error: any) {
      setIsSuccess(false);
      if (axios.isAxiosError(error) && error.response) {
        // Lỗi từ server (ví dụ: 401 Unauthorized, 400 Bad Request)
        setMessage(error.response.data.message || 'Email hoặc mật khẩu không đúng.');
      } else if (error.request) {
        // Không nhận được phản hồi từ server (ví dụ: server không chạy, lỗi mạng)
        setMessage('Không nhận được phản hồi từ server. Vui lòng kiểm tra kết nối mạng hoặc URL API.');
        console.error('Lỗi request:', error.request);
      } else {
        // Lỗi khác trong quá trình gửi request
        setMessage('Đã xảy ra lỗi: ' + error.message);
        console.error('Lỗi:', error.message);
      }
      console.error('Chi tiết lỗi đăng nhập:', error);
    }
  };

  // Đóng modal khi click vào lớp phủ bên ngoài
  const handleOverlayClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  // Chuyển hướng đến trang đăng ký
  const handleGoToRegister = () => {
    onClose(); // Đóng modal hiện tại trước khi chuyển hướng
    navigate('/register'); // Chuyển hướng đến đường dẫn '/register'
  };

  return (
    // Overlay của modal (lớp phủ đen)
    <div
      style={{
        position: 'fixed',
        top: 0,
        left: 0,
        width: '100vw',
        height: '100vh',
        backgroundColor: 'rgba(0, 0, 0, 0.7)',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        zIndex: 20000, // Z-index rất cao để đảm bảo modal nằm trên tất cả
      }}
      onClick={handleOverlayClick}
    >
      {/* Container của form đăng nhập */}
      <div style={{
        backgroundColor: '#fff',
        padding: '30px 40px',
        borderRadius: '12px',
        boxShadow: '0 8px 30px rgba(0,0,0,0.3)',
        maxWidth: '450px',
        width: '90%',
        position: 'relative',
        animation: 'fadeInScale 0.3s ease-out forwards',
        zIndex: 20001, // Cao hơn lớp phủ
      }}>
        {/* Nút đóng modal */}
        <button
          onClick={onClose}
          style={{
            position: 'absolute',
            top: '15px',
            right: '15px',
            background: 'none',
            border: 'none',
            fontSize: '1.5rem',
            cursor: 'pointer',
            color: '#888',
            lineHeight: '1',
          }}
        >
          &times;
        </button>

        <h2 style={{ textAlign: 'center', marginBottom: '25px', color: '#007bff' }}>Đăng Nhập</h2>

        {/* Khu vực hiển thị thông báo lỗi/thành công */}
        {message && (
          <div style={{
            color: isSuccess ? '#28a745' : '#dc3545',
            marginBottom: '20px',
            border: `1px solid ${isSuccess ? '#28a745' : '#dc3545'}`,
            padding: '12px',
            borderRadius: '6px',
            backgroundColor: isSuccess ? '#d4edda' : '#f8d7da',
            fontSize: '0.95rem'
          }}>
            {message}
          </div>
        )}

        {/* Form nhập liệu */}
        <div style={{ marginBottom: '20px' }}>
          <label htmlFor="modalLoginEmail" style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', color: '#555' }}>Email</label>
          <input
            type="email"
            id="modalLoginEmail"
            placeholder='Nhập Email'
            value={email}
            onChange={handleEmailChange}
            style={{ width: 'calc(100% - 22px)', padding: '12px', border: '1px solid #ccc', borderRadius: '6px', boxSizing: 'border-box', fontSize: '1rem' }}
          />
        </div>

        <div style={{ marginBottom: '25px' }}>
          <label htmlFor="modalLoginPassword" style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', color: '#555' }}>Mật khẩu</label>
          <input
            type="password"
            id="modalLoginPassword"
            placeholder='Nhập mật khẩu'
            value={password}
            onChange={handlePasswordChange}
            style={{ width: 'calc(100% - 22px)', padding: '12px', border: '1px solid #ccc', borderRadius: '6px', boxSizing: 'border-box', fontSize: '1rem' }}
          />
        </div>

        {/* Nút Đăng nhập */}
        <button
          onClick={handleLogin}
          style={{
            width: '100%',
            padding: '14px 20px',
            backgroundColor: '#007bff',
            color: 'white',
            border: 'none',
            borderRadius: '8px',
            cursor: 'pointer',
            fontSize: '1.1rem',
            fontWeight: 'bold',
            transition: 'background-color 0.3s ease',
            marginBottom: '15px'
          }}
        >
          Đăng Nhập
        </button>

        {/* Nút/Liên kết Đăng ký */}
        <p style={{ textAlign: 'center', fontSize: '0.95rem', color: '#666', marginTop: '15px' }}>
          Chưa có tài khoản?{' '}
          <button
            onClick={handleGoToRegister}
            style={{
              background: 'none',
              border: 'none',
              color: '#007bff',
              cursor: 'pointer',
              fontSize: '0.95rem',
              textDecoration: 'underline',
              padding: '0',
            }}
          >
            Đăng ký ngay
          </button>
        </p>

        {/* CSS cho animation (inline style) */}
        <style>
          {`
            @keyframes fadeInScale {
              from {
                opacity: 0;
                transform: scale(0.9);
              }
              to {
                opacity: 1;
                transform: scale(1);
              }
            }
          `}
        </style>
      </div>
    </div>
  );
};

export default LoginModal;