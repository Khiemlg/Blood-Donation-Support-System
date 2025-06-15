import React from 'react';
import { Link } from 'react-router-dom';
import Header from '../components/Header/Header';
import Navbar from '../components/Navbar/Navbar';
import Footer from '../components/Footer/Footer';

function NotFoundPage() {
    return (
        <div className="page-wrapper">
            <Header />
            <Navbar />
            <main className="container my-5 text-center">
                <h1 className="display-1 text-danger">404</h1>
                <h2 className="mb-4">Trang không tìm thấy</h2>
                <p className="lead">Rất tiếc, trang bạn đang tìm kiếm không tồn tại.</p>
                <Link to="/" className="btn btn-primary mt-3">Quay về Trang chủ</Link>
            </main>
            <Footer />
        </div>
    );
}

export default NotFoundPage;
