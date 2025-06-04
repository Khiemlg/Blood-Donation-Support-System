// src/Pages/HomePage.tsx
import React, { useState } from 'react';
import { useAuth } from '../Context/AuthContext.tsx';
import { useNavigate } from 'react-router-dom';
import LoginModal from '../Component/LoginModal.tsx';
import Navbar from '../Component/Navbar.tsx';

const HomePage: React.FC = () => {
  const { user } = useAuth(); // Chỉ cần user để hiển thị nội dung tùy thuộc trạng thái đăng nhập
  const navigate = useNavigate(); // Vẫn giữ navigate nếu có chuyển hướng khác trong HomePage
  const [showLoginModal, setShowLoginModal] = useState(false); // State để quản lý hiển thị modal

  // Hàm xử lý khi người dùng click Đăng nhập (sẽ mở modal)
  const handleOpenLoginModal = () => {
    setShowLoginModal(true);
  };

  // Hàm để đóng modal
  const handleCloseLoginModal = () => {
    setShowLoginModal(false);
  };

  // Hàm được gọi khi đăng nhập thành công từ modal
  const handleLoginSuccess = () => {
    console.log("Đăng nhập thành công từ modal!");
    // Modal sẽ tự đóng thông qua onClose() được gọi từ LoginModal
    // AuthContext đã tự cập nhật user, HomePage sẽ render lại theo user mới.
  };

  return (
    <div style={{
      width: '100vw', // Chiếm 100% chiều rộng của viewport
      minHeight: '100vh', // Chiếm tối thiểu 100% chiều cao của viewport, cho phép nội dung dài hơn cuộn
      padding: '0', // Đảm bảo không có padding làm tràn ra ngoài
      background: '#f0f2f5', // Màu nền nhẹ cho toàn bộ trang
      position: 'relative', // Quan trọng cho việc định vị các phần tử con
      overflowY: 'auto', // Thêm thanh cuộn dọc nếu nội dung vượt quá chiều cao
      boxSizing: 'border-box', // Đảm bảo padding không làm tăng kích thước tổng thể
      display: 'flex', // Sử dụng flexbox để căn giữa nội dung
      flexDirection: 'column', // Xếp các mục theo cột
      alignItems: 'center', // Căn giữa theo chiều ngang
      justifyContent: 'flex-start', // Bắt đầu nội dung từ phía trên
    }}>
      {/* Navbar cố định trên cùng */}
      <Navbar onOpenLoginModal={handleOpenLoginModal} />

      {/* Nội dung chính của trang chủ */}
      {/* Div này chứa nội dung chính, có giới hạn chiều rộng và được căn giữa */}
      <div style={{
        maxWidth: '960px', // Vẫn giới hạn chiều rộng nội dung để dễ đọc
        width: '100%', // Đảm bảo chiếm 100% của maxWidth
        backgroundColor: '#fff', // Nền trắng cho phần nội dung
        padding: '30px', // Padding bên trong nội dung
        borderRadius: '10px', // Bo góc
        boxShadow: '0 5px 20px rgba(0,0,0,0.1)', // Đổ bóng
        marginTop: '80px', // Khoảng cách từ trên cùng để không bị navbar cố định che mất (điều chỉnh nếu Navbar cao hơn/thấp hơn)
        marginBottom: '20px', // Khoảng cách dưới cùng
      }}>
        {/* Tiêu đề chính của trang hiến máu */}
        <h2 style={{ textAlign: 'center', marginBottom: '30px', color: '#cc0000', fontSize: '2.5rem', paddingTop: '0' }}>
          <span style={{ color: '#007bff' }}>Mỗi giọt máu</span> - Một cuộc đời
        </h2>

        {user ? (
          // **********************************************
          // NỘI DUNG KHI NGƯỜI DÙNG ĐÃ ĐĂNG NHẬP
          // **********************************************
          <div style={{ fontSize: '1.2rem', lineHeight: '1.8', textAlign: 'center' }}>
            <p>Chào mừng, <strong style={{ color: '#28a745' }}>{user.username}</strong>!</p>
            <p>Email của bạn: <strong style={{ color: '#6c757d' }}>{user.email}</strong></p>
            <p>Vai trò của bạn: <strong style={{ color: '#ffc107' }}>{user.role}</strong></p>
            <p>Cảm ơn bạn đã sẵn sàng lan tỏa yêu thương. Hãy cùng tham gia các hoạt động hiến máu sắp tới!</p>

            <button
              onClick={() => alert("Tính năng tìm lịch hiến máu đang phát triển!")}
              style={{
                marginTop: '30px',
                padding: '12px 25px',
                backgroundColor: '#28a745',
                color: 'white',
                border: 'none',
                borderRadius: '8px',
                cursor: 'pointer',
                fontSize: '1.1rem',
                fontWeight: 'bold',
                transition: 'background-color 0.3s ease'
              }}
            >
              Tìm lịch hiến máu gần nhất
            </button>
          </div>
        ) : (
          // **********************************************
          // NỘI DUNG KHI NGƯỜI DÙNG CHƯA ĐĂNG NHẬP (chủ đề hiến máu)
          // **********************************************
          <div style={{ textAlign: 'center', fontSize: '1.1rem', lineHeight: '1.6' }}>
            <p style={{ color: '#555', fontSize: '1.3rem' }}>
              Hiến máu cứu người là một hành động cao cả, mang lại sự sống cho những người kém may mắn.
            </p>
            <p style={{ color: '#777', marginBottom: '40px' }}>
              Hãy cùng chúng tôi lan tỏa yêu thương và xây dựng một cộng đồng khỏe mạnh hơn.
            </p>
            <img
              src="https://via.placeholder.com/600x300/e6e6e6/000000?text=Hình+ảnh+minh+họa+hiến+máu" // Ảnh minh họa hiến máu
              alt="Hiến máu cứu người"
              style={{ maxWidth: '100%', height: 'auto', borderRadius: '10px', marginBottom: '30px', border: '1px solid #ddd' }}
            />
            <p style={{ color: '#0056b3', fontSize: '1.2rem', fontWeight: 'bold' }}>
              Để tham gia hoặc tìm hiểu thêm về các chương trình hiến máu, vui lòng đăng nhập!
            </p>
          </div>
        )}
      </div>

      {/* Render LoginModal nếu showLoginModal là true */}
      {showLoginModal && (
        <LoginModal
          onClose={handleCloseLoginModal}
          onLoginSuccess={handleLoginSuccess}
        />
      )}
    </div>
  );
};

export default HomePage;