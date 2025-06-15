import React, { createContext, useState, useEffect, useCallback } from 'react';
import * as authService from '../services/authService'; // Sử dụng * as để import tất cả exports
import { jwtDecode } from 'jwt-decode';

export const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState(null); // Lưu thông tin người dùng đã giải mã từ token

  // Hàm trợ giúp để lấy vai trò từ decodedToken, xử lý các tên claim khác nhau
  const getRoleFromDecodedToken = (token) => {
    // Thử lấy 'role' theo tên mặc định hoặc tên tùy chỉnh nếu có
    let role = token.role || token['role'] || token.Role; 
    
    // Nếu vẫn không tìm thấy, thử lấy theo URI chuẩn của ClaimTypes.Role trong .NET
    // Đây là ClaimTypes.Role của .NET
    if (!role) {
      role = token['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    }
    
    // Trả về vai trò hoặc 'User' nếu không tìm thấy để tránh lỗi undefined
    return role || 'User'; 
  };

  // Hàm cập nhật trạng thái đăng nhập, được bọc trong useCallback để đảm bảo ổn định
  const updateAuthStatus = useCallback(() => {
    const token = localStorage.getItem('jwtToken');
    if (token) {
      try {
        const decodedToken = jwtDecode(token);
        const currentTime = Date.now() / 1000;

        if (decodedToken.exp < currentTime) {
          console.log("Token đã hết hạn.");
          authService.logout();
          setIsAuthenticated(false);
          setUser(null);
        } else {
          setIsAuthenticated(true);
          setUser({ // Trích xuất thông tin cần thiết từ decodedToken
            userId: decodedToken.user_id, // Đảm bảo claim 'user_id' được cấu hình trong JWT ở Backend
            username: decodedToken.unique_name, // 'unique_name' thường là username
            email: decodedToken.email,
            role: getRoleFromDecodedToken(decodedToken) // Sử dụng hàm trợ giúp để lấy vai trò
          });
        }
      } catch (error) {
        // Lỗi giải mã token hoặc token không hợp lệ
        console.error("Lỗi giải mã token hoặc truy cập thuộc tính trong updateAuthStatus:", error);
        authService.logout(); // Xóa token lỗi
        setIsAuthenticated(false);
        setUser(null);
      }
    } else {
      setIsAuthenticated(false);
      setUser(null);
    }
  }, []); // Dependencies rỗng vì nó chỉ phụ thuộc vào các setters (ổn định) và localStorage

  // Hàm đăng nhập, được bọc trong useCallback để đảm bảo ổn định
  const login = useCallback(async (email, password) => {
    try {
      // authService.login giờ đây trả về TRỰC TIẾP CHUỖI TOKEN
      const token = await authService.login(email, password); 
      localStorage.setItem('jwtToken', token); // Lưu token nhận được

      const decodedToken = jwtDecode(token); // Giải mã token để lấy thông tin user

      setIsAuthenticated(true);
      setUser({
        userId: decodedToken.user_id,
        username: decodedToken.unique_name,
        email: decodedToken.email,
        role: getRoleFromDecodedToken(decodedToken)
      });
      // Trả về một đối tượng chứa thông tin user đã giải mã cho component gọi (Login.jsx)
      return { token: token, user: {
        userId: decodedToken.user_id,
        username: decodedToken.unique_name,
        email: decodedToken.email,
        role: getRoleFromDecodedToken(decodedToken)
      }};
    } catch (error) {
      console.error("Lỗi đăng nhập AuthContext:", error);
      setIsAuthenticated(false);
      setUser(null);
      throw error; // Ném lỗi để component gọi có thể xử lý
    }
  }, []); // Dependencies rỗng vì nó chỉ phụ thuộc vào authService và các setters (đã ổn định)

  // Hàm đăng xuất
  const logout = useCallback(() => { // Cũng bọc logout trong useCallback
    authService.logout();
    setIsAuthenticated(false);
    setUser(null);
  }, []); // Dependencies rỗng

  // Lắng nghe sự kiện 'authChange' và kiểm tra trạng thái khi component mount
  useEffect(() => {
    updateAuthStatus(); // Kiểm tra trạng thái khi component mount

    window.addEventListener('authChange', updateAuthStatus);

    return () => {
      window.removeEventListener('authChange', updateAuthStatus);
    };
  }, [updateAuthStatus]); // Đã thêm updateAuthStatus vào dependency array

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
