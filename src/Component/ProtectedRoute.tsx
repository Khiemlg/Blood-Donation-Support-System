// src/components/ProtectedRoute.tsx
import React, { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../Context/AuthContext.tsx';

interface ProtectedRouteProps {
  children: ReactNode;
  allowedRoles?: string[]; // Optional: Nếu bạn muốn bảo vệ theo vai trò
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, allowedRoles }) => {
  const { user } = useAuth();

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && user.role && !allowedRoles.includes(user.role)) {
    alert(`Bạn không có quyền truy cập trang này. Vai trò của bạn là: ${user.role}`);
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;