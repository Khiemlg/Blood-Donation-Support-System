// src/services/userService.js
import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor để thêm token vào mỗi request (trừ các request public)
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('jwtToken');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// --- USER PROFILE API CALLS ---

/**
 * Lấy thông tin hồ sơ người dùng bằng UserId.
 * Yêu cầu token (user có thể lấy profile của chính mình).
 * @param {string} userId
 * @returns {Promise<object>} UserProfileDto
 */
export const getUserProfileByUserId = async (userId) => {
    try {
        const response = await api.get(`/UserProfile/by-user/${userId}`);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể lấy thông tin hồ sơ người dùng.';
        console.error('Error fetching user profile:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Cập nhật thông tin hồ sơ người dùng.
 * Yêu cầu token.
 * @param {string} userId
 * @param {object} updateProfileDto
 * @returns {Promise<object>} UserProfileDto đã cập nhật
 */
export const updateProfile = async (userId, updateProfileDto) => {
    try {
        const response = await api.put(`/UserProfile/by-user/${userId}`, updateProfileDto);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể cập nhật hồ sơ người dùng.';
        console.error('Error updating user profile:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Tạo hồ sơ người dùng mới. (Có thể chỉ Admin hoặc tạo tự động sau khi đăng ký)
 * @param {object} createProfileDto
 * @returns {Promise<object>} UserProfileDto đã tạo
 */
export const createUserProfile = async (createProfileDto) => {
    try {
        const response = await api.post('/UserProfile', createProfileDto);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể tạo hồ sơ người dùng.';
        console.error('Error creating user profile:', errorMessage);
        throw new Error(errorMessage);
    }
};


// --- ADMIN: MANAGE USER ACCOUNTS API CALLS ---

/**
 * [ADMIN ONLY] Lấy tất cả người dùng.
 * Yêu cầu Admin token.
 * @returns {Promise<Array<object>>} Danh sách UserDto
 */
export const getAllUsers = async () => {
    try {
        const response = await api.get('/ManageUserAccounts');
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data || 'Không thể lấy danh sách người dùng.';
        console.error('Error fetching all users (Admin):', errorMessage);
        throw new Error(errorMessage.message || errorMessage);
    }
};

/**
 * [ADMIN ONLY] Lấy người dùng theo ID.
 * Yêu cầu Admin token.
 * @param {string} userId
 * @returns {Promise<object>} UserDto
 */
export const getUserById = async (userId) => {
    try {
        const response = await api.get(`/ManageUserAccounts/${userId}`);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data || 'Không tìm thấy người dùng.';
        console.error('Error fetching user by ID (Admin):', errorMessage);
        throw new Error(errorMessage.message || errorMessage);
    }
};

/**
 * [ADMIN ONLY] Tạo người dùng mới (có thể có hoặc không cần OTP).
 * Yêu cầu Admin token.
 * @param {object} createUserDto
 * @returns {Promise<object>} UserDto đã tạo
 */
export const createUserByAdmin = async (createUserDto) => {
    try {
        const response = await api.post('/ManageUserAccounts', createUserDto);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data || 'Không thể tạo người dùng mới.';
        console.error('Error creating user (Admin):', errorMessage);
        throw new Error(errorMessage.message || errorMessage);
    }
};

/**
 * [ADMIN ONLY] Cập nhật thông tin người dùng.
 * Yêu cầu Admin token.
 * @param {string} userId
 * @param {object} updateDto
 * @returns {Promise<object>} UserDto đã cập nhật
 */
export const updateUser = async (userId, updateDto) => {
    try {
        const response = await api.put(`/ManageUserAccounts/${userId}`, updateDto);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data || 'Không thể cập nhật người dùng.';
        console.error('Error updating user (Admin):', errorMessage);
        throw new Error(errorMessage.message || errorMessage);
    }
};

/**
 * [ADMIN ONLY] Cập nhật vai trò (role) của người dùng.
 * Yêu cầu Admin token.
 * @param {string} userId
 * @param {string} roleName
 * @returns {Promise<object>} UserDto với role đã cập nhật
 */
export const updateUserRole = async (userId, roleName) => {
    try {
        const response = await api.put(`/ManageUserAccounts/${userId}/role`, { roleName });
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data || 'Không thể cập nhật vai trò người dùng.';
        console.error('Error updating user role (Admin):', errorMessage);
        throw new Error(errorMessage.message || errorMessage);
    }
};

/**
 * [ADMIN ONLY] Xóa người dùng.
 * Yêu cầu Admin token.
 * @param {string} userId
 * @returns {Promise<string>} Message thành công
 */
export const deleteUser = async (userId) => {
    try {
        await api.delete(`/ManageUserAccounts/${userId}`);
        return 'User deleted successfully.';
    } catch (error) {
        const errorMessage = error.response?.data || 'Không thể xóa người dùng.';
        console.error('Error deleting user (Admin):', errorMessage);
        throw new Error(errorMessage.message || errorMessage);
    }
};