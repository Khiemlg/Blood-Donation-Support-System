// src/App.tsx
import React from 'react';
import AppRoutes from './Routes/AppRoutes.tsx'; // Import AppRoutes từ thư mục Routes của bạn
import './App.css'; // Giữ lại nếu bạn có file CSS toàn cục cho App

function App() {
  return (
    <div className="App">
      {/* AppRoutes sẽ chứa tất cả các định tuyến (HomePage, LoginPage, RegisterPage, v.v.) */}
      <AppRoutes />
    </div>
  );
}

export default App;