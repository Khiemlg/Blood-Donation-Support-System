// src/Context/AuthContext.tsx
import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';

// Định nghĩa kiểu cho thông tin người dùng
interface User {
  userId: string;
  username: string;
  email: string;
  role: string; // Đảm bảo thuộc tính 'role' có trong User interface
}

// Định nghĩa kiểu cho AuthContext
interface AuthContextType {
  token: string | null;
  user: User | null;
  login: (token: string, user: User) => void;
  logout: () => void;
  isAuthenticated: boolean;
}

// Tạo Context
const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Props cho AuthProvider
interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  // Khởi tạo state từ Session Storage (thay vì Local Storage)
  const [token, setToken] = useState<string | null>(
    sessionStorage.getItem('jwt_token')
  );
  const [user, setUser] = useState<User | null>(() => {
    const storedUser = sessionStorage.getItem('user');
    return storedUser ? JSON.parse(storedUser) : null;
  });

  const isAuthenticated = !!token; // Kiểm tra xem có token hay không để xác định trạng thái đăng nhập

  // Hàm đăng nhập: Lưu token và user vào Session Storage
  const login = (newToken: string, newUser: User) => {
    setToken(newToken);
    setUser(newUser);
    sessionStorage.setItem('jwt_token', newToken);
    sessionStorage.setItem('user', JSON.stringify(newUser));
  };

  // Hàm đăng xuất: Xóa token và user khỏi Session Storage
  const logout = () => {
    setToken(null);
    setUser(null);
    sessionStorage.removeItem('jwt_token');
    sessionStorage.removeItem('user');
  };

  // useEffect để đồng bộ state với Session Storage khi có thay đổi (chủ yếu cho lần tải trang đầu tiên)
  useEffect(() => {
    const storedToken = sessionStorage.getItem('jwt_token');
    const storedUser = sessionStorage.getItem('user');

    if (storedToken && storedUser) {
      setToken(storedToken);
      try {
        setUser(JSON.parse(storedUser));
      } catch (e) {
        console.error("Failed to parse user from session storage", e);
        // Xóa dữ liệu nếu không thể parse để tránh lỗi liên tục
        logout();
      }
    } else {
      // Nếu không có token hoặc user, đảm bảo state là null
      setToken(null);
      setUser(null);
    }
  }, []); // [] đảm bảo effect chỉ chạy một lần khi component mount

  return (
    <AuthContext.Provider value={{ token, user, login, logout, isAuthenticated }}>
      {children}
    </AuthContext.Provider>
  );
};

// Custom hook để sử dụng AuthContext
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};