import React from 'react';
import Header from '../components/Header/Header';
import Navbar from '../components/Navbar/Navbar';
import Footer from '../components/Footer/Footer';

function BlogPage() {
    return (
        <div className="page-wrapper">
            <Header />
            <Navbar />
            <main className="container my-5">
                <h2 className="text-center text-danger mb-4">Blog - Góc Chia Sẻ</h2>
                <p className="text-center lead">
                    Nơi tổng hợp các bài viết, câu chuyện, và tin tức về hiến máu.
                </p>

                <div className="row g-4 mt-4">
                    <div className="col-md-6">
                        <div className="card shadow-sm h-100">
                            <div className="card-body">
                                <h5 className="card-title">Bài viết 1: Lợi ích của việc hiến máu đối với sức khỏe</h5>
                                <p className="card-text text-muted small">Ngày đăng: 01/06/2025</p>
                                <p className="card-text">
                                    Hiến máu không chỉ giúp người khác mà còn mang lại nhiều lợi ích sức khỏe cho chính người hiến.
                                </p>
                                <a href="#" className="btn btn-outline-primary btn-sm">Đọc toàn bộ</a>
                            </div>
                        </div>
                    </div>
                    <div className="col-md-6">
                        <div className="card shadow-sm h-100">
                            <div className="card-body">
                                <h5 className="card-title">Bài viết 2: Giải mã những lầm tưởng về hiến máu</h5>
                                <p className="card-text text-muted small">Ngày đăng: 28/05/2025</p>
                                <p className="card-text">
                                    Có rất nhiều quan niệm sai lầm về hiến máu. Bài viết này sẽ giúp bạn hiểu rõ hơn.
                                </p>
                                <a href="#" className="btn btn-outline-primary btn-sm">Đọc toàn bộ</a>
                            </div>
                        </div>
                    </div>
                    {/* Thêm các bài viết blog khác tại đây */}
                </div>
            </main>
            <Footer />
        </div>
    );
}

export default BlogPage;
