// src/hooks/useAuth.js
import { useContext } from 'react';
import { AuthContext } from '../context/AuthContext'; // Đảm bảo đường dẫn đúng

/**
 * Custom hook to access authentication context.
 * Provides currentUser, isAuthenticated, isAdmin, isStaff, isMember, login, logout, and isAuthReady.
 */
const useAuth = () => {
  const context = useContext(AuthContext);

  // Throw an error if useAuth is used outside of an AuthProvider
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }

  return context;
};

export default useAuth;
