// src/Component/Navbar.tsx
import React from 'react';
import { Link } from 'react-router-dom';
import UserMenu from './UserMenu.tsx';

interface NavbarProps {
  onOpenLoginModal: () => void;
}

const Navbar: React.FC<NavbarProps> = ({ onOpenLoginModal }) => {
  return (
    <nav style={{
      width: '98%',
      backgroundColor: '#f8f9fa',
      padding: '15px 10px',
      boxShadow: '0 2px 10px rgba(0,0,0,0.1)',
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      position: 'fixed', // Giữ navbar cố định ở trên cùng
      top: 0,
      left: 0,
      zIndex: 10000, // <--- Tăng Z-INDEX của Navbar lên một giá trị RẤT CAO
    }}>
      {/* Logo hoặc tên ứng dụng */}
      <div style={{ fontSize: '1.8rem', fontWeight: 'bold', color: '#cc0000' }}>
        <Link to="/" style={{ textDecoration: 'none', color: '#cc0000' }}>
          Blood<span style={{ color: '#007bff' }}>Life</span>
        </Link>
      </div>

      {/* Các liên kết điều hướng */}
      <div style={{ display: 'flex', alignItems: 'center' }}>
        <Link to="/blog" style={navLinkStyle}>Blog</Link>
        <Link to="/materials" style={navLinkStyle}>Tài liệu hiến máu</Link>
      </div>

      {/* User Menu */}
      <UserMenu onOpenLoginModal={onOpenLoginModal} />
    </nav>
  );
};

const navLinkStyle: React.CSSProperties = {
  textDecoration: 'none',
  color: '#333',
  fontSize: '1.1rem',
  fontWeight: '600',
  marginLeft: '25px',
  transition: 'color 0.3s ease',
};

export default Navbar;