import React from 'react';
// Đảm bảo đường dẫn CSS này đúng với vị trí file Shared.css của bạn
import '../../styles/Shared.css';

function LoadingSpinner() {
  return (
    <div className="loading-spinner-container">
      <div className="spinner-border text-primary" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
      <p className="loading-text">Đang tải...</p>
    </div>
  );
}

export default LoadingSpinner;
