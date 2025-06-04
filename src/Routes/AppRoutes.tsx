// src/Routes/AppRoutes.tsx
import React from 'react';
import { Routes, Route } from 'react-router-dom';
import RegisterPage from '../Pages/RegisterPage.tsx';
import HomePage from '../Pages/HomePage.tsx';

// Tạo các component tạm thời cho Blog và Materials nếu bạn chưa có
const BlogPage: React.FC = () => (
  <div style={{ paddingTop: '80px', textAlign: 'center' }}>
    <h2>Trang Blog đang phát triển!</h2>
    <p>Nơi chia sẻ kiến thức về hiến máu.</p>
  </div>
);
const MaterialsPage: React.FC = () => (
  <div style={{ paddingTop: '80px', textAlign: 'center' }}>
    <h2>Tài liệu Hiến máu</h2>
    <p>Các tài liệu và hướng dẫn cần thiết.</p>
  </div>
);


const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/" element={<HomePage />} />
      <Route path="/blog" element={<BlogPage />} /> {/* Thêm route cho Blog */}
      <Route path="/materials" element={<MaterialsPage />} /> {/* Thêm route cho Materials */}

      {/* CÁC TRANG KHÁC CẦN BẢO VỆ */}
      {/* Ví dụ:
      <Route path="/dashboard" element={<ProtectedRoute><YourDashboardComponent /></ProtectedRoute>} />
      */}
    </Routes>
  );
};

export default AppRoutes;