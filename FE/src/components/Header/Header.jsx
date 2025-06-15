import React from 'react';
import { Link } from 'react-router-dom';
import './Header.css'
// Không cần import Heart từ 'lucide-react' nữa nếu bạn dùng ảnh logo

function Header() {
    return (
        // Đã thay đổi className thành "header-custom" để sử dụng CSS tùy chỉnh
        <header className="header-custom"> 
            <div className="container-custom"> {/* Thêm container để căn giữa nội dung */}
                {/* Logo và Tên Hệ thống */}
                <Link to="/" className="logo-link"> {/* Thêm className cho Link */}
                    {/* Sử dụng thẻ <img> với đường dẫn tới LOGOBlood.png */}
                    {/* Đảm bảo LOGOBlood.png nằm trong public/assets/ */}
                    <img 
                        src="/assets/LOGOBlood.png" 
                        alt="BloodLife Logo" 
                        className="header-logo-img" // Thêm className để định kiểu bằng CSS
                        onError={(e) => { 
                            e.target.onerror = null; 
                            e.target.src = "https://placehold.co/32x32/FF0000/FFFFFF?text=LOGO"; // Cập nhật kích thước placeholder
                            console.error("Failed to load LOGOBlood.png. Using placeholder.");
                        }}
                    />
                    <span className="header-title">
                        BloodLife
                    </span>
                </Link>
            </div>
        </header>
    );
}

export default Header;
