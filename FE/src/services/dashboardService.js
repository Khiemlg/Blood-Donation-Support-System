// src/services/dashboardService.js
import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

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

/**
 * [ADMIN ONLY] Lấy tóm tắt dashboard.
 * Yêu cầu Admin token.
 * @returns {Promise<object>} DashboardSummaryDto
 */
export const getDashboardSummary = async () => {
    try {
        const response = await api.get('/Dashboard/summary');
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể lấy tóm tắt dashboard.';
        console.error('Error fetching dashboard summary (Admin):', errorMessage);
        throw new Error(errorMessage);
    }
};

// src/services/roleService.js
// Các hàm tương tác với RoleController (chỉ Admin)

/**
 * [ADMIN ONLY] Lấy tất cả các vai trò.
 * @returns {Promise<Array<object>>} Danh sách RoleDto
 */
export const getAllRoles = async () => {
    try {
        const response = await api.get('/Role');
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể lấy tất cả vai trò.';
        console.error('Error fetching all roles (Admin):', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * [ADMIN ONLY] Lấy vai trò theo ID.
 * @param {number} roleId
 * @returns {Promise<object>} RoleDto
 */
export const getRoleById = async (roleId) => {
    try {
        const response = await api.get(`/Role/${roleId}`);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không tìm thấy vai trò.';
        console.error('Error fetching role by ID (Admin):', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * [ADMIN ONLY] Tạo vai trò mới.
 * @param {object} createRoleDto - { RoleName: string, Description: string }
 * @returns {Promise<object>} RoleDto đã tạo
 */
export const createRole = async (createRoleDto) => {
    try {
        const response = await api.post('/Role', createRoleDto);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể tạo vai trò mới.';
        console.error('Error creating role (Admin):', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * [ADMIN ONLY] Cập nhật vai trò.
 * @param {number} roleId
 * @param {object} updateRoleDto - { RoleName: string, Description: string }
 * @returns {Promise<object>} RoleDto đã cập nhật
 */
export const updateRole = async (roleId, updateRoleDto) => {
    try {
        const response = await api.put(`/Role/${roleId}`, updateRoleDto);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể cập nhật vai trò.';
        console.error('Error updating role (Admin):', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * [ADMIN ONLY] Xóa vai trò.
 * @param {number} roleId
 * @returns {Promise<void>}
 */
export const deleteRole = async (roleId) => {
    try {
        await api.delete(`/Role/${roleId}`);
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể xóa vai trò.';
        console.error('Error deleting role (Admin):', errorMessage);
        throw new Error(errorMessage);
    }
};