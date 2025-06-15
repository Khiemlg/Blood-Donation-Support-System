import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

// Thay đổi URL này để khớp với API của bạn (ví dụ: https://localhost:7080/api)
// Đảm bảo đây là URL chính xác của API Backend của bạn.
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'https://localhost:44387/api'; // KIỂM TRA LẠI DÒNG NÀY RẤT QUAN TRỌNG

// Tạo một instance Axios để dễ dàng cấu hình headers (ví dụ: cho Authorization)
const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor để thêm token vào mỗi request (trừ các request public)
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('jwtToken');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Interceptor để xử lý lỗi response, ví dụ: token hết hạn (401)
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    // Nếu lỗi 401 (Unauthorized) và không phải là request login/register,
    // và chưa thử lại (để tránh vòng lặp vô hạn)
    if (error.response?.status === 401 && !originalRequest._retry && !originalRequest.url.includes('/Auth')) {
      originalRequest._retry = true;
      localStorage.removeItem('jwtToken');
      window.dispatchEvent(new Event('authChange'));
      window.location.href = '/login'; 
      return Promise.reject(error);
    }
    return Promise.reject(error);
  }
);


// --- AUTHENTICATION API CALLS ---

/**
 * Gửi mã OTP đến email để đăng ký.
 * @param {string} email
 * @returns {Promise<string>} Message from API
 */
export const sendOtpForRegistration = async (email) => {
    try {
        const response = await api.post('/Auth/send-otp', { email });
        return response.data.message; // "Mã OTP đã được gửi tới email."
    } catch (error) {
        const errorMessage = error.response?.data?.error || 'Gửi OTP thất bại.';
        console.error('Error sending OTP for registration:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Đăng ký tài khoản mới với OTP.
 * @param {string} username
 * @param {string} email
 * @param {string} password
 * @param {string} confirmPassword
 * @param {string} otpCode
 * @returns {Promise<string>} JWT Token
 */
export const register = async (username, email, password, confirmPassword, otpCode) => {
    try {
        const response = await api.post('/Auth/register', { username, email, password, confirmPassword, otpCode });
        // API trả về TokenDto { Token: "..." }
        return response.data.token;
    } catch (error) {
        const errorMessage = error.response?.data?.error || 'Đăng ký thất bại. Vui lòng kiểm tra thông tin hoặc mã OTP.';
        console.error('Error during registration:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Đăng nhập người dùng và nhận JWT Token.
 * @param {string} email
 * @param {string} password
 * @returns {Promise<string>} JWT Token
 */
export const login = async (email, password) => {
    try {
        const response = await api.post('/Auth/login', { email, password });
        // API Backend của bạn trả về TokenDto { Token: "..." }
        if (response.data && response.data.token) {
            return response.data.token; // TRẢ VỀ CHỈ TOKEN
        }
        throw new Error(response.data?.error || 'Phản hồi không chứa token.');
    } catch (error) {
        const errorMessage = error.response?.data?.error || 'Đăng nhập thất bại. Vui lòng kiểm tra email và mật khẩu.';
        console.error('Error during login:', errorMessage);
        throw new Error(errorMessage);
    }
};

// --- RESET PASSWORD API CALLS ---

/**
 * Gửi mã OTP để đặt lại mật khẩu.
 * @param {string} email
 * @returns {Promise<string>} Message from API
 */
export const sendOtpForResetPassword = async (email) => {
    try {
        const response = await api.post('/Auth/forgot-password', { email }); // Đã sửa về Auth/forgot-password theo AuthController
        return response.data.message; // API trả về "OTP sent successfully."
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Gửi OTP thất bại. Email không tồn tại hoặc lỗi hệ thống.';
        console.error('Error sending OTP for password reset:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Đặt lại mật khẩu bằng OTP.
 * @param {string} email
 * @param {string} otpCode
 * @param {string} newPassword
 * @param {string} confirmNewPassword
 * @returns {Promise<string>} Message from API
 */
export const resetPassword = async (email, otpCode, newPassword, confirmNewPassword) => {
    try {
        const response = await api.post('/Auth/reset-password', { email, otpCode, newPassword, confirmNewPassword }); // Đã sửa về Auth/reset-password theo AuthController
        return response.data.message; // API trả về "Password reset successful."
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Đặt lại mật khẩu thất bại. Mã OTP không hợp lệ/hết hạn hoặc lỗi hệ thống.';
        console.error('Error resetting password:', errorMessage);
        throw new Error(errorMessage);
    }
};

// Hàm giúp giải mã JWT token (cần thiết để lấy userId và role)
export const decodeToken = (token) => {
    try {
        return jwtDecode(token); 
    } catch (error) {
        console.error("Error decoding token:", error);
        return null;
    }
};

// Hàm để đăng xuất (xóa token khỏi localStorage)
export const logout = () => {
    localStorage.removeItem('jwtToken');
    window.dispatchEvent(new Event('authChange'));
};

// Hàm để lấy thông tin người dùng hiện tại từ token nếu có (sử dụng trong AuthContext và utils)
export const getCurrentUser = () => {
    const token = localStorage.getItem('jwtToken');
    if (token) {
        return jwtDecode(token);
    }
    return null;
};
