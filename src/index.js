// src/index.js (hoặc main.tsx)
import { createRoot } from 'react-dom/client'
import './index.css' // Nếu có file này
import App from './App.tsx' // <--- RẤT QUAN TRỌNG: Phải là './App.tsx'
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './Context/AuthContext.tsx';
import React from 'react'; // Cần thiết cho JSX/TSX

createRoot(document.getElementById('root')).render(
    <BrowserRouter>
        <React.StrictMode>
            <AuthProvider>
                <App />
            </AuthProvider>
        </React.StrictMode>
    </BrowserRouter>
);