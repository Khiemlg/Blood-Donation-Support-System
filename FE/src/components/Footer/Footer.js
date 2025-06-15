
import React from 'react';
import './Footer.css'; // Tạo file CSS này sau

function Footer() {
    return (
        <footer className="footer">
            <p>&copy; 2025 Hệ thống Hiến máu. Mọi quyền được bảo lưu.</p>
            <div className="contact-info">
                <p>Địa chỉ: Đường A, Phường B, Quận C, TP.HCM</p>
                <p>Email: contact@blooddonation.com</p>
                <p>Điện thoại: 0123 456 789</p>
            </div>
        </footer>
    );
}

export default Footer;