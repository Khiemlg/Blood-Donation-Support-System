// src/Component/UserMenu.tsx
import React, { useState, useRef, useEffect } from 'react';
import { useAuth } from '../Context/AuthContext.tsx';
import { useNavigate } from 'react-router-dom';

interface UserMenuProps {
  onOpenLoginModal: () => void;
}

const UserMenu: React.FC<UserMenuProps> = ({ onOpenLoginModal }) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [menuRef]);

  const handleLogout = () => {
    logout();
    navigate('/');
    setIsOpen(false);
  };

  const handleProfileClick = () => {
    alert("Tính năng User Profile đang phát triển!");
    setIsOpen(false);
  };

  const handleLoginClick = () => {
    onOpenLoginModal();
    setIsOpen(false);
  };

  return (
    // Đảm bảo div này không có position: relative hoặc absolute nếu nó nằm trong một flex container
    // Nếu nó nằm trong Navbar là flex container, nó sẽ tự động được căn chỉnh.
    // Z-index của div này không cần quá cao trừ khi nó có position riêng.
    <div style={{ position: 'relative' }}> {/* Giữ position: relative để menu dropdown hoạt động */}
      {user ? (
        // Nút tròn khi đã đăng nhập
        <button
          onClick={() => setIsOpen(!isOpen)}
          style={{
            width: '45px',
            height: '45px',
            borderRadius: '50%',
            backgroundColor: '#007bff',
            color: 'white',
            border: '2px solid #fff',
            cursor: 'pointer',
            fontSize: '1.2rem',
            fontWeight: 'bold',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            boxShadow: '0 2px 10px rgba(0,0,0,0.2)',
            transition: 'background-color 0.3s ease',
          }}
          title={user.username || user.email}
        >
          {user.username ? user.username.charAt(0).toUpperCase() : (user.email ? user.email.charAt(0).toUpperCase() : '?')}
        </button>
      ) : (
        // Nút "Đăng nhập" khi chưa đăng nhập
        <button
          onClick={handleLoginClick}
          style={{
            padding: '10px 15px',
            backgroundColor: '#007bff',
            color: 'white',
            border: 'none',
            borderRadius: '5px',
            cursor: 'pointer',
            fontSize: '1rem',
            fontWeight: 'bold',
            transition: 'background-color 0.3s ease'
          }}
        >
          Đăng Nhập
        </button>
      )}

      {/* Menu thả xuống */}
      {isOpen && user && (
        <div style={{
          position: 'absolute',
          top: 'calc(100% + 10px)',
          right: '0',
          backgroundColor: '#fff',
          borderRadius: '8px',
          boxShadow: '0 4px 15px rgba(0,0,0,0.2)',
          minWidth: '160px',
          padding: '10px 0',
          zIndex: 10001, // <--- Z-INDEX cao hơn Navbar
          display: 'flex',
          flexDirection: 'column',
        }}>
          <div style={{ padding: '10px 15px', borderBottom: '1px solid #eee', color: '#555', fontSize: '0.9rem' }}>
            Chào mừng, <strong>{user.username || user.email}</strong>
          </div>
          <button
            onClick={handleProfileClick}
            style={{
              background: 'none',
              border: 'none',
              padding: '10px 15px',
              textAlign: 'left',
              cursor: 'pointer',
              fontSize: '1rem',
              color: '#333',
              whiteSpace: 'nowrap',
              transition: 'background-color 0.2s ease',
            }}
            onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#f0f0f0'}
            onMouseOut={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
          >
            Thông tin cá nhân
          </button>
          <button
            onClick={handleLogout}
            style={{
              background: 'none',
              border: 'none',
              padding: '10px 15px',
              textAlign: 'left',
              cursor: 'pointer',
              fontSize: '1rem',
              color: '#e74c3c',
              whiteSpace: 'nowrap',
              transition: 'background-color 0.2s ease',
            }}
            onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#f0f0f0'}
            onMouseOut={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
          >
            Đăng xuất
          </button>
        </div>
      )}
    </div>
  );
};

export default UserMenu;