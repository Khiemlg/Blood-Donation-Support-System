// src/Pages/RegisterPage.tsx
import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { registerUser } from '../service/serviceauth.tsx'; // <-- CẬP NHẬT ĐƯỜNG DẪN

const RegisterPage = () => {
    const [username, setUsername] = useState<string>('');
    const [email, setEmail] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [message, setMessage] = useState<string>('');
    const [isSuccess, setIsSuccess] = useState<boolean | null>(null);

    const navigate = useNavigate();

    const handleUsernameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setUsername(e.target.value);
    };
    const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setEmail(e.target.value);
    };
    const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(e.target.value);
    };

    const handleSave = async () => {
        setMessage('');
        setIsSuccess(null);

        if (!username || !email || !password) {
            setMessage('Vui lòng nhập đầy đủ tên người dùng, email và mật khẩu.');
            setIsSuccess(false);
            return;
        }

        try {
            const result = await registerUser(username, email, password);
            
            setMessage(result.data.message || 'Đăng ký thành công! Vui lòng đăng nhập.');
            setIsSuccess(true);
            
            setUsername('');
            setEmail('');
            setPassword('');

            setTimeout(() => {
                navigate('/login');
            }, 2000);

        } catch (error: any) {
            setIsSuccess(false);
            if (axios.isAxiosError(error) && error.response) {
                setMessage(error.response.data.message || 'Lỗi đăng ký không xác định từ server.');
            } else if (error.request) {
                setMessage('Không nhận được phản hồi từ server. Vui lòng kiểm tra kết nối mạng hoặc URL API.');
            } else {
                setMessage('Đã xảy ra lỗi: ' + error.message);
            }
            console.error('Lỗi đăng ký:', error);
        }
    };

    const handleGoLoginClick = () => {
        navigate('/login');
    };

    return (
        <div style={{ padding: '20px', maxWidth: '450px', margin: '50px auto', border: '1px solid #e0e0e0', borderRadius: '10px', boxShadow: '0 5px 20px rgba(0,0,0,0.1)', background: '#fff' }}>
            <h2 style={{ textAlign: 'center', marginBottom: '25px', color: '#007bff' }}>Đăng Ký Tài Khoản</h2>

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

            <div style={{ marginBottom: '20px' }}>
                <label htmlFor="txtName" style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', color: '#555' }}>Tên người dùng</label>
                <input
                    type="text"
                    id="txtName"
                    placeholder='Nhập tên người dùng'
                    value={username}
                    onChange={handleUsernameChange}
                    style={{ width: 'calc(100% - 22px)', padding: '12px', border: '1px solid #ccc', borderRadius: '6px', boxSizing: 'border-box', fontSize: '1rem' }}
                />
            </div>

            <div style={{ marginBottom: '20px' }}>
                <label htmlFor="txtEmail" style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', color: '#555' }}>Email</label>
                <input
                    type="email"
                    id="txtEmail"
                    placeholder='Nhập Email'
                    value={email}
                    onChange={handleEmailChange}
                    style={{ width: 'calc(100% - 22px)', padding: '12px', border: '1px solid #ccc', borderRadius: '6px', boxSizing: 'border-box', fontSize: '1rem' }}
                />
            </div>

            <div style={{ marginBottom: '25px' }}>
                <label htmlFor="txtPassword" style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', color: '#555' }}>Mật khẩu</label>
                <input
                    type="password"
                    id="txtPassword"
                    placeholder='Nhập mật khẩu'
                    value={password}
                    onChange={handlePasswordChange}
                    style={{ width: 'calc(100% - 22px)', padding: '12px', border: '1px solid #ccc', borderRadius: '6px', boxSizing: 'border-box', fontSize: '1rem' }}
                />
            </div>

            <button
                onClick={handleSave}
                style={{
                    width: '100%',
                    padding: '14px 20px',
                    backgroundColor: '#28a745',
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
                Đăng Ký
            </button>

            <p style={{ marginTop: '15px', textAlign: 'center', fontSize: '14px' }}>
                Đã có tài khoản?{' '}
                <span
                    onClick={handleGoLoginClick}
                    style={{ color: '#007bff', textDecoration: 'underline', cursor: 'pointer' }}
                >
                    Đăng nhập ngay
                </span>
            </p>
        </div>
    );
};

export default RegisterPage;