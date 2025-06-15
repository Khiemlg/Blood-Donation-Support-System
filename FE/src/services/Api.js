import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'https://localhost:44387/api'; // Thay đổi nếu BE chạy trên cổng khác

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Thêm interceptor để đính kèm JWT token vào mỗi request (trừ request đăng nhập/đăng ký)
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('jwtToken'); // Lấy token từ localStorage
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Thêm interceptor để xử lý lỗi response, ví dụ: token hết hạn
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Nếu lỗi 401 Unauthorized (token hết hạn hoặc không hợp lệ)
    if (error.response && error.response.status === 401) {
      console.log('Token hết hạn hoặc không hợp lệ. Đang chuyển hướng đến trang đăng nhập...');
      // Xóa token cũ và chuyển hướng về trang đăng nhập
      localStorage.removeItem('jwtToken');
      // Không cần removeItem('currentUser') nếu AuthContext đã quản lý tốt
      window.location.href = '/login'; // Chuyển hướng cứng, có thể dùng navigate của react-router-dom
    }
    return Promise.reject(error);
  }
);

export default api;
