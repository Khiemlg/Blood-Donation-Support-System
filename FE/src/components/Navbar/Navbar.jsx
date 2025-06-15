import React, { useState, useRef, useEffect } from 'react'; // Đã bỏ useContext khỏi destructuring
import { Link, useNavigate } from 'react-router-dom';
import { AuthContext } from '../../context/AuthContext'; // Import AuthContext
import { User, LogOut, Settings } from 'lucide-react'; // Import icons cho user profile
import './Navbar.css'; // File CSS cho Navbar

function Navbar() {
    // Sử dụng React.useContext thay vì useContext trực tiếp để tránh cảnh báo linter
    const { isAuthenticated, logout, user } = React.useContext(AuthContext); 
    const navigate = useNavigate();
    const [isProfileMenuOpen, setIsProfileMenuOpen] = useState(false);
    const profileMenuRef = useRef(null); // Ref để đóng dropdown khi click ra ngoài

    const handleLogout = () => {
        logout(); // Gọi hàm logout từ AuthContext
        setIsProfileMenuOpen(false); // Đóng menu sau khi đăng xuất
        navigate('/login'); // Điều hướng về trang đăng nhập
    };

    const handleViewProfile = () => {
        setIsProfileMenuOpen(false); // Đóng menu
        navigate('/profile'); // Điều hướng đến trang hồ sơ người dùng (cần tạo route này)
    };

    // Đóng dropdown khi click ra ngoài
    useEffect(() => {
        const handleClickOutside = (event) => {
            if (profileMenuRef.current && !profileMenuRef.current.contains(event.target)) {
                setIsProfileMenuOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    return (
        <nav className="navbar-custom">
            <div className="navbar-content-wrapper">
                {/* Phần điều hướng chính (căn giữa) */}
                <ul className='nav-links-center'>
                    <li><Link to="/" className="nav-item-link">Trang chủ</Link></li>
                    <li><Link to="/blog" className="nav-item-link">Blog</Link></li>
                    <li><Link to="/documentation" className="nav-item-link">Tài liệu</Link></li>
                    {isAuthenticated && (
                        <>
                            {/* Bạn có thể thêm các link dashboard tùy theo role ở đây nếu muốn */}
                            <li><Link to="/dashboard" className="nav-item-link">Dashboard</Link></li>
                        </>
                    )}
                </ul>

                {/* Phần nút đăng nhập/profile (căn phải) */}
                <div className="navbar-actions">
                    {isAuthenticated ? (
                        <div className="user-profile-wrapper" ref={profileMenuRef}>
                            <button 
                                className="user-avatar-button" 
                                onClick={() => setIsProfileMenuOpen(!isProfileMenuOpen)}
                                aria-expanded={isProfileMenuOpen}
                                aria-label="Menu người dùng"
                            >
                                <User size={24} color="#dc3545" strokeWidth={2.5} /> {/* Icon người dùng */}
                                {/* Hiển thị tên người dùng nếu có */}
                                {user && <span className="user-name-display d-none d-md-inline ms-2">{user.username || user.email}</span>}
                            </button>
                            {isProfileMenuOpen && (
                                <div className="profile-dropdown-menu">
                                    <button onClick={handleViewProfile} className="dropdown-item">
                                        <Settings size={18} className="me-2"/> Hồ sơ của tôi
                                    </button>
                                    <button onClick={handleLogout} className="dropdown-item">
                                        <LogOut size={18} className="me-2"/> Đăng xuất
                                    </button>
                                </div>
                            )}
                        </div>
                    ) : (
                        <Link to="/login" className="btn btn-login">Đăng nhập</Link>
                    )}
                </div>
            </div>
        </nav>
    );
}

export default Navbar;
