import React from 'react';
import { Link } from 'react-router-dom';
import Header from '../../components/Header/Header';
import Navbar from '../../components/Navbar/Navbar';
import Footer from '../../components/Footer/Footer';
import useAuth from '../../hooks/useAuth'; // Import useAuth hook
import './HomePage.css';

function HomePage() {
    const { isAuthenticated, isMember } = useAuth(); // Lấy trạng thái xác thực và vai trò thành viên

    return (
        <div className="homepage-wrapper">
            <Header />
            <Navbar />
            <main>
                {/* Hero Section */}
                <section className="hero-section text-center text-white d-flex align-items-center justify-content-center">
                    <div className="container">
                        <h1 className="display-4 mb-3">Hiến Máu Cứu Người, Trao Sự Sống</h1>
                        <p className="lead mb-4">
                            Mỗi giọt máu cho đi là một cuộc đời ở lại. Hãy tham gia cùng chúng tôi để tạo nên sự khác biệt.
                        </p>
                        <div className="hero-buttons">
                            {/* Nút đăng ký/hiến máu sẽ thay đổi tùy theo trạng thái đăng nhập */}
                            {isAuthenticated && isMember ? (
                                <Link to="/member/register-donation" className="btn btn-danger btn-lg me-3">Đăng Ký Hiến Máu Ngay</Link>
                            ) : (
                                <Link to="/register" className="btn btn-danger btn-lg me-3">Đăng Ký Thành Viên</Link>
                            )}
                            <Link to="/documentation" className="btn btn-outline-light btn-lg">Tìm Hiểu Thêm</Link>
                        </div>
                    </div>
                </section>

                {/* Member-Specific Section (Conditional Rendering) */}
                {isAuthenticated && isMember && (
                    <section className="member-dashboard-summary py-5 bg-light">
                        <div className="container text-center">
                            <h2 className="mb-4 text-primary">Chào mừng Thành viên!</h2>
                            <p className="lead mb-5">
                                Các chức năng dành riêng cho bạn:
                            </p>
                            <div className="row g-4 justify-content-center">
                                <div className="col-md-4">
                                    <div className="card h-100 shadow-sm member-feature-card">
                                        <div className="card-body">
                                            <h5 className="card-title">Cập nhật hồ sơ</h5>
                                            <p className="card-text">Quản lý thông tin cá nhân, nhóm máu, địa chỉ, tình trạng sức khỏe.</p>
                                            <Link to="/member/profile" className="btn btn-outline-primary btn-sm mt-3">Cập nhật ngay</Link>
                                        </div>
                                    </div>
                                </div>
                                <div className="col-md-4">
                                    <div className="card h-100 shadow-sm member-feature-card">
                                        <div className="card-body">
                                            <h5 className="card-title">Đăng ký hiến máu</h5>
                                            <p className="card-text">Đăng ký ngày sẵn sàng hiến máu, nhóm máu, thành phần hiến.</p>
                                            <Link to="/member/register-donation" className="btn btn-outline-danger btn-sm mt-3">Đăng ký</Link>
                                        </div>
                                    </div>
                                </div>
                                <div className="col-md-4">
                                    <div className="card h-100 shadow-sm member-feature-card">
                                        <div className="card-body">
                                            <h5 className="card-title">Lịch sử hiến máu</h5>
                                            <p className="card-text">Tra cứu các lần hiến máu đã thực hiện và kết quả xét nghiệm.</p>
                                            <Link to="/member/donation-history" className="btn btn-outline-success btn-sm mt-3">Xem lịch sử</Link>
                                        </div>
                                    </div>
                                </div>
                                <div className="col-md-4">
                                    <div className="card h-100 shadow-sm member-feature-card">
                                        <div className="card-body">
                                            <h5 className="card-title">Thông báo khẩn cấp</h5>
                                            <p className="card-text">Nhận yêu cầu hiến máu khẩn cấp và phản hồi kịp thời.</p>
                                            <Link to="/member/notifications" className="btn btn-outline-warning btn-sm mt-3">Kiểm tra thông báo</Link>
                                        </div>
                                    </div>
                                </div>
                                <div className="col-md-4">
                                    <div className="card h-100 shadow-sm member-feature-card">
                                        <div className="card-body">
                                            <h5 className="card-title">Nhắc nhở phục hồi</h5>
                                            <p className="card-text">Nhận thông báo khi đủ điều kiện hiến máu lại.</p>
                                            <Link to="/member/reminders" className="btn btn-outline-info btn-sm mt-3">Xem nhắc nhở</Link>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="mt-5">
                                <Link to="/member/dashboard" className="btn btn-danger btn-lg">Đi tới Trang tổng quan Thành viên</Link>
                            </div>
                        </div>
                    </section>
                )}

                {/* About Us Section - unchanged */}
                <section className="about-section py-5">
                    <div className="container">
                        <div className="row justify-content-center">
                            <div className="col-lg-10 text-center">
                                <h2 className="mb-4 text-dark">Về Chúng Tôi</h2>
                                <p className="lead">
                                    Chúng tôi là **Bệnh viện Đa khoa Quốc tế ABC**, một cơ sở y tế hàng đầu cam kết cung cấp dịch vụ chăm sóc sức khỏe chất lượng cao. Với sự hiểu biết sâu sắc về tầm quan trọng của nguồn máu, chúng tôi đã phát triển hệ thống quản lý hiến máu nội bộ để đảm bảo nguồn cung cấp máu ổn định và an toàn cho mọi bệnh nhân.
                                </p>
                                <p className="mb-5">
                                    Tại đây, mỗi đơn vị máu hiến tặng đều được quản lý chặt chẽ, từ khâu tiếp nhận, xét nghiệm đến lưu trữ và sử dụng. Sứ mệnh của chúng tôi là không chỉ điều trị mà còn kêu gọi và tạo điều kiện thuận lợi nhất để cộng đồng cùng chúng tôi lan tỏa nghĩa cử cao đẹp của việc hiến máu, góp phần cứu sống nhiều sinh mạng.
                                </p>
                                <div className="about-features-grid row g-4">
                                    <div className="col-md-4">
                                        <div className="feature-card shadow-sm">
                                            <div className="feature-icon-wrapper feature-icon-safety">
                                                <svg className="feature-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                                    <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path>
                                                </svg>
                                            </div>
                                            <h5 className="feature-title">An Toàn Tuyệt Đối</h5>
                                            <p className="feature-description">
                                                Quy trình hiến máu tuân thủ nghiêm ngặt các tiêu chuẩn y tế quốc tế, đảm bảo an toàn tối đa cho cả người hiến và người nhận.
                                            </p>
                                        </div>
                                    </div>
                                    <div className="col-md-4">
                                        <div className="feature-card shadow-sm">
                                            <div className="feature-icon-wrapper feature-icon-professional">
                                                <svg className="feature-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                                    <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
                                                    <polyline points="22 4 12 14.01 9 11.01"></polyline>
                                                </svg>
                                            </div>
                                            <h5 className="feature-title">Chuyên Nghiệp</h5>
                                            <p className="feature-description">
                                                Đội ngũ y bác sĩ giàu kinh nghiệm, trang thiết bị hiện đại và dịch vụ chăm sóc tận tình trong suốt quá trình hiến máu.
                                            </p>
                                        </div>
                                    </div>
                                    <div className="col-md-4">
                                        <div className="feature-card shadow-sm">
                                            <div className="feature-icon-wrapper feature-icon-response">
                                                <svg className="feature-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                                    <circle cx="12" cy="12" r="10"></circle>
                                                    <polyline points="12 6 12 12 16 14"></polyline>
                                                </svg>
                                            </div>
                                            <h5 className="feature-title">Phản Ứng Nhanh</h5>
                                            <p className="feature-description">
                                                Hệ thống tiếp nhận và phân phối máu nhanh chóng, hiệu quả, đảm bảo nguồn máu sẵn sàng khi bệnh nhân cần.
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>

                {/* Knowledge Section - Start (unchanged) */}
                <section className="knowledge-section py-5">
                    <div className="container">
                        <h2 className="text-center mb-5 text-danger">Kiến Thức Về Hiến Máu</h2>
                        <div className="process-flow-section row justify-content-center mb-5">
                            <div className="col-md-3 col-sm-6 process-step-col">
                                <div className="card h-100 shadow-sm process-step">
                                    <div className="step-header">
                                        <div className="step-number-wrapper">
                                            <span className="step-number-text">Bước</span>
                                            <span className="step-number">1</span>
                                        </div>
                                        <div className="step-arrow-wrapper d-none d-md-block">
                                            <svg className="step-arrow" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                                                <path d="M14 5l7 7-7 7M3 12h18"/>
                                            </svg>
                                        </div>
                                    </div>
                                    <div className="card-body text-center">
                                        <div className="step-icon-wrapper">
                                            <svg className="step-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                                                <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>
                                            </svg>
                                        </div>
                                        <h5 className="step-title">Đăng Ký & Kiểm Tra</h5>
                                        <p className="step-description">Điền form đăng ký và kiểm tra sức khỏe ban đầu</p>
                                        <ul className="step-detail-list text-start">
                                            <li><span className="bullet-point"></span> Kiểm tra tuổi (18-60 tuổi)</li>
                                            <li><span className="bullet-point"></span> Cân nặng tối thiểu 45kg</li>
                                            <li><span className="bullet-point"></span> Đo huyết áp và mạch</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-3 col-sm-6 process-step-col">
                                <div className="card h-100 shadow-sm process-step">
                                    <div className="step-header">
                                        <div className="step-number-wrapper">
                                            <span className="step-number-text">Bước</span>
                                            <span className="step-number">2</span>
                                        </div>
                                        <div className="step-arrow-wrapper d-none d-md-block">
                                            <svg className="step-arrow" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                                                <path d="M14 5l7 7-7 7M3 12h18"/>
                                            </svg>
                                        </div>
                                    </div>
                                    <div className="card-body text-center">
                                        <div className="step-icon-wrapper">
                                            <svg className="step-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                                                <path d="M19 3h-4.18C14.4 1.84 13.3 1 12 1s-2.4.84-2.82 2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-6 15h-2v-2h2v2zm0-4h-2V8h2v6zm-1-9c-.55 0-1-.45-1-1s.45-1 1-1 1 .45 1 1-.45 1-1 1z"/>
                                            </svg>
                                        </div>
                                        <h5 className="step-title">Khám Sàng Lọc</h5>
                                        <p className="step-description">Bác sĩ thăm khám và tư vấn chi tiết</p>
                                        <ul className="step-detail-list text-start">
                                            <li><span className="bullet-point"></span> Hỏi tiền sử bệnh</li>
                                            <li><span className="bullet-point"></span> Kiểm tra mức hemoglobin</li>
                                            <li><span className="bullet-point"></span> Tư vấn quy trình hiến máu</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-3 col-sm-6 process-step-col">
                                <div className="card h-100 shadow-sm process-step">
                                    <div className="step-header">
                                        <div className="step-number-wrapper">
                                            <span className="step-number-text">Bước</span>
                                            <span className="step-number">3</span>
                                        </div>
                                        <div className="step-arrow-wrapper d-none d-md-block">
                                            <svg className="step-arrow" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                                                <path d="M14 5l7 7-7 7M3 12h18"/>
                                            </svg>
                                        </div>
                                    </div>
                                    <div className="card-body text-center">
                                        <div className="step-icon-wrapper">
                                            <svg className="step-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                                                <path d="M12 21.35l-1.84-1.66C4.48 15.37 2 13.07 2 10.5 2 7.55 4.55 5 7.5 5c1.74 0 3.41.81 4.5 2.09C13.09 5.81 14.76 5 16.5 5 19.45 5 22 7.55 22 10.5c0 2.57-2.48 4.87-8.16 9.14L12 21.35z"/>
                                            </svg>
                                        </div>
                                        <h5 className="step-title">Hiến Máu</h5>
                                        <p className="step-description">Thực hiện hiến máu trong môi trường vô trùng</p>
                                        <ul className="step-detail-list text-start">
                                            <li><span className="bullet-point"></span> Thời gian: 8-10 phút</li>
                                            <li><span className="bullet-point"></span> Lượng máu: 350-450ml</li>
                                            <li><span className="bullet-point"></span> An toàn tuyệt đối</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-3 col-sm-6 process-step-col">
                                <div className="card h-100 shadow-sm process-step">
                                    <div className="step-header">
                                        <div className="step-number-wrapper">
                                            <span className="step-number-text">Bước</span>
                                            <span className="step-number">4</span>
                                        </div>
                                    </div>
                                    <div className="card-body text-center">
                                        <div className="step-icon-wrapper">
                                            <svg className="step-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                                                <path d="M20 3H4c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H4V5h16v14zM6 7h8v2H6zm0 4h8v2H6zm0 4h5v2H6z"/>
                                            </svg>
                                        </div>
                                        <h5 className="step-title">Nghỉ Ngơi & Chăm Sóc</h5>
                                        <p className="step-description">Nghỉ ngơi và chăm sóc sức khỏe sau hiến máu</p>
                                        <ul className="step-detail-list text-start">
                                            <li><span className="bullet-point"></span> Nghỉ ngơi 15-20 phút</li>
                                            <li><span className="bullet-point"></span> Uống nước và ăn nhẹ</li>
                                            <li><span className="bullet-point"></span> Theo dõi sức khỏe</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="row g-4 mt-5">
                            <div className="col-md-6">
                                <div className="card h-100 shadow-sm">
                                    <div className="card-body text-center">
                                        <h5 className="card-title text-primary mb-4">Thông tin nhóm máu</h5>
                                        <div className="blood-types-grid">
                                            <div className="blood-type-card shadow-sm">
                                                <span className="blood-type-text text-danger">O+</span>
                                                <p className="blood-type-detail">Hiến cho: Mọi nhóm Rh+</p>
                                                <p className="blood-type-detail">Nhận từ: O+, O-</p>
                                            </div>
                                            <div className="blood-type-card shadow-sm">
                                                <span className="blood-type-text text-danger">O-</span>
                                                <p className="blood-type-detail">Hiến cho: Mọi nhóm</p>
                                                <p className="blood-type-detail">Nhận từ: O-</p>
                                            </div>
                                            <div className="blood-type-card shadow-sm">
                                                <span className="blood-type-text text-danger">A+</span>
                                                <p className="blood-type-detail">Hiến cho: A+, AB+</p>
                                                <p className="blood-type-detail">Nhận từ: A+, A-, O+, O-</p>
                                            </div>
                                            <div className="blood-type-card shadow-sm">
                                                <span className="blood-type-text text-danger">A-</span>
                                                <p className="blood-type-detail">Hiến cho: A+, A-, AB+, AB-</p>
                                                <p className="blood-type-detail">Nhận từ: A-, O-</p>
                                            </div>
                                            <div className="blood-type-card shadow-sm">
                                                <span className="blood-type-text text-danger">B+</span>
                                                <p className ="blood-type-detail">Hiến cho: B+, AB+</p>
                                                <p className="blood-type-detail">Nhận từ: B+, B-, O+, O-</p>
                                            </div>
                                            <div className="blood-type-card shadow-sm">
                                                <span className="blood-type-text text-danger">B-</span>
                                                <p className="blood-type-detail">Hiến cho: B+, B-, AB+, AB-</p>
                                                <p className="blood-type-detail">Nhận từ: B-, O-</p>
                                            </div>
                                            <div className="blood-type-card shadow-sm">
                                                <span className="blood-type-text text-danger">AB+</span>
                                                <p className="blood-type-detail">Hiến cho: AB+</p>
                                                <p className="blood-type-detail">Nhận từ: Mọi nhóm</p>
                                            </div>
                                            <div className="blood-type-card shadow-sm">
                                                <span className="blood-type-text text-danger">AB-</span>
                                                <p className="blood-type-detail">Hiến cho: AB+, AB-</p>
                                                <p className="blood-type-detail">Nhận từ: A-, B-, AB-, O-</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-6">
                                <h5 className="text-center text-primary mb-4">Lợi ích của việc hiến máu</h5>
                                <div className="benefits-grid-section row g-3">
                                    <div className="col-sm-6 col-lg-6">
                                        <div className="benefit-card shadow-sm">
                                            <div className="benefit-icon-wrapper benefit-icon-heart">
                                                <svg className="benefit-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                                    <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"></path>
                                                </svg>
                                            </div>
                                            <h5 className="benefit-title">Cải Thiện Sức Khỏe Tim Mạch</h5>
                                            <p className="benefit-description">Hiến máu giúp giảm nguy cơ mắc bệnh tim mạch và đột quỵ bằng cách giảm độ nhớt của máu.</p>
                                        </div>
                                    </div>
                                    <div className="col-sm-6 col-lg-6">
                                        <div className="benefit-card shadow-sm">
                                            <div className="benefit-icon-wrapper benefit-icon-pulse">
                                                <svg className="benefit-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                                    <path d="M12 20v-6M12 14c-1.5 0-3-.5-4-2M12 14c1.5 0 3-.5 4-2M12 20L8 16M12 20L16 16M16 8l-4-4-4 4"></path>
                                                    <path d="M8 8s1.5-1.5 4-1.5 4 1.5 4 1.5"></path>
                                                    <path d="M12 12s-1.5 1.5-4 1.5-4-1.5-4-1.5"></path>
                                                </svg>
                                            </div>
                                            <h5 className="benefit-title">Kích Thích Tạo Máu Mới</h5>
                                            <p className="benefit-description">Sau khi hiến máu, cơ thể sẽ tự động sản xuất tế bào máu mới khỏe mạnh hơn.</p>
                                        </div>
                                    </div>
                                    <div className="col-sm-6 col-lg-6">
                                        <div className="benefit-card shadow-sm">
                                            <div className="benefit-icon-wrapper benefit-icon-check">
                                                <svg className="benefit-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                                    <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
                                                    <polyline points="22 4 12 14.01 9 11.01"></polyline>
                                                </svg>
                                            </div>
                                            <h5 className="benefit-title">Kiểm Tra Sức Khỏe Miễn Phí</h5>
                                            <p className="benefit-description">Được kiểm tra sức khỏe tổng quát và xét nghiệm máu miễn phí trước mỗi lần hiến.</p>
                                        </div>
                                    </div>
                                    <div className="col-sm-6 col-lg-6">
                                        <div className="benefit-card shadow-sm">
                                            <div className="benefit-icon-wrapper benefit-icon-shield">
                                                <svg className="benefit-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                                    <path d="M18.36 6.64a9 9 0 1 1-12.73 0"></path>
                                                    <line x1="12" y1="2" x2="12" y2="12"></line>
                                                </svg>
                                            </div>
                                            <h5 className="benefit-title">Giảm Nguy Cơ Ung Thư</h5>
                                            <p className="benefit-description">Nghiên cứu cho thấy hiến máu đều đặn có thể giảm nguy cơ mắc một số loại ung thư.</p>
                                        </div>
                                    </div>
                                    <div className="col-sm-6 col-lg-6">
                                        <div className="benefit-card shadow-sm">
                                            <div className="benefit-icon-wrapper benefit-icon-users">
                                                <svg className="benefit-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                                    <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
                                                    <circle cx="9" cy="7" r="4"></circle>
                                                    <path d="M23 21v-2a4 4 0 0 0-3-3.87"></path>
                                                    <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
                                                </svg>
                                            </div>
                                            <h5 className="benefit-title">Tác Động Xã Hội Tích Cực</h5>
                                            <p className="benefit-description">Mỗi lần hiến máu có thể cứu sống đến 3 người, tạo ra ý nghĩa sống to lớn.</p>
                                        </div>
                                    </div>
                                    <div className="col-sm-6 col-lg-6">
                                        <div className="benefit-card shadow-sm">
                                            <div className="benefit-icon-wrapper benefit-icon-smile">
                                                <svg className="benefit-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                                    <circle cx="12" cy="12" r="10"></circle>
                                                    <path d="M8 14s1.5 2 4 2 4-2 4-2"></path>
                                                    <line x1="9" y1="9" x2="9.01" y2="9"></line>
                                                    <line x1="15" y1="9" x2="15.01" y2="9"></line>
                                                </svg>
                                            </div>
                                            <h5 className="benefit-title">Cảm Giác Hạnh Phúc</h5>
                                            <p className="benefit-description">Việc giúp đỡ người khác sẽ tạo ra cảm giác hạnh phúc và hài lòng trong tâm hồn.</p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
                {/* Knowledge Section - End */}

                {/* Blog/Sharing Section - unchanged */}
                <section className="blog-section py-5 bg-light">
                    <div className="container">
                        <h2 className="text-center mb-5 text-danger">Góc Chia Sẻ Từ Cộng Đồng</h2>
                        <div className="row g-4">
                            <div className="col-md-6 col-lg-4">
                                <div className="card h-100 shadow-sm">
                                    <img src="/assets/blog1.jpg" onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/400x250/cccccc/333333?text=Blog+Image+1"; }} className="card-img-top" alt="Bài viết Blog 1" />
                                    <div className="card-body">
                                        <h5 className="card-title text-primary">Hành Trình Của Một Người Hiến Máu Thường Xuyên</h5>
                                        <p className="card-text text-muted small">Ngày đăng: 10/06/2025</p>
                                        <p className="card-text">
                                            Câu chuyện cảm động về anh Nguyễn Văn A, người đã hiến máu 30 lần và truyền cảm hứng cho cộng đồng.
                                        </p>
                                        <Link to="/blog/post-1" className="btn btn-outline-primary btn-sm">Đọc thêm</Link>
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-6 col-lg-4">
                                <div className="card h-100 shadow-sm">
                                    <img src="/assets/blog2.jpg" onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/400x250/cccccc/333333?text=Blog+Image+2"; }} className="card-img-top" alt="Bài viết Blog 2" />
                                    <div className="card-body">
                                        <h5 className="card-title text-primary">Hiến Máu Lần Đầu: Những Điều Bạn Cần Biết</h5>
                                        <p className="card-text text-muted small">Ngày đăng: 01/06/2025</p>
                                        <p className="card-text">
                                            Bài viết tổng hợp các câu hỏi thường gặp và giải đáp thắc mắc cho người mới hiến máu.
                                        </p>
                                        <Link to="/blog/post-2" className="btn btn-outline-primary btn-sm">Đọc thêm</Link>
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-6 col-lg-4">
                                <div className="card h-100 shadow-sm">
                                    <img src="/assets/blog3.jpg" onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/400x250/cccccc/333333?text=Blog+Image+3"; }} className="card-img-top" alt="Bài viết Blog 3" />
                                    <div className="card-body">
                                        <h5 className="card-title text-primary">Tầm Quan Trọng Của Máu Hiếm Trong Y Học</h5>
                                        <p className="card-text text-muted small">Ngày đăng: 25/05/2025</p>
                                        <p className="card-text">
                                            Tìm hiểu về các nhóm máu hiếm và vai trò sống còn của những người hiến máu đặc biệt.
                                        </p>
                                        <Link to="/blog/post-3" className="btn btn-outline-primary btn-sm">Đọc thêm</Link>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="text-center mt-5">
                            <Link to="/blog" className="btn btn-outline-danger btn-lg">Xem tất cả bài viết</Link>
                        </div>
                    </div>
                </section>

                {/* Call to Action Section - unchanged */}
                <section className="call-to-action-section bg-danger text-white py-5 text-center">
                    <div className="container">
                        <h2 className="mb-4 display-5">Bạn đã sẵn sàng trở thành người hùng?</h2>
                        <p className="lead mb-4">
                            Hiến máu không chỉ là một hành động nhân ái, mà còn là cơ hội để bạn tạo nên sự khác biệt.
                        </p>
                        <Link to="/register" className="btn btn-light btn-lg">Đăng Ký Ngay</Link>
                    </div>
                </section>
            </main>
            <Footer />
        </div>
    );
}

export default HomePage;
