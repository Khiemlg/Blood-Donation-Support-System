import { jwtDecode } from 'jwt-decode'; // Đã sửa import từ jwt_decode thành jwtDecode

export const decodeToken = (token) => {
  try {
    return jwtDecode(token); // Đã sửa tên hàm jwtDecode
  } catch (error) {
    console.error('Error decoding token:', error);
    return null;
  }
};

export const getToken = () => {
  return localStorage.getItem('jwtToken');
};

export const getAuthUser = () => {
  const token = getToken();
  if (token) {
    const decoded = decodeToken(token);
    if (decoded && decoded.exp * 1000 > Date.now()) { // Kiểm tra token còn hạn
      // Lưu ý: Các claim 'user_id', 'username', 'email', 'role'
      // cần khớp với cách backend của bạn tạo JWT.
      return {
        userId: decoded.user_id, // Giả định claim user_id
        username: decoded.username || decoded.sub, // 'sub' thường là username nếu không có 'username'
        email: decoded.email,
        role: decoded.role, // Đảm bảo claim 'role' tồn tại
        exp: decoded.exp,
      };
    }
  }
  return null;
};

export const isAuthenticated = () => {
  const user = getAuthUser();
  return !!user;
};

export const isAdmin = () => {
  const user = getAuthUser();
  return user && user.role === 'Admin';
};

export const isStaff = () => {
  const user = getAuthUser();
  return user && user.role === 'Staff';
};

export const isMember = () => {
  const user = getAuthUser();
  return user && user.role === 'Member';
};
