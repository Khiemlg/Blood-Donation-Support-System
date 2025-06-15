// src/services/donationService.js
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

// --- DONATION HISTORY API CALLS ---

/**
 * Lấy tất cả lịch sử hiến máu.
 * @returns {Promise<Array<object>>} Danh sách DonationHistoryDetailDto
 */
export const getAllDonationHistories = async () => {
    try {
        const response = await api.get('/DonationHistory');
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể lấy tất cả lịch sử hiến máu.';
        console.error('Error fetching all donation histories:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Lấy lịch sử hiến máu theo ID.
 * @param {string} donationId
 * @returns {Promise<object>} DonationHistoryDetailDto
 */
export const getDonationHistoryById = async (donationId) => {
    try {
        const response = await api.get(`/DonationHistory/${donationId}`);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không tìm thấy lịch sử hiến máu.';
        console.error('Error fetching donation history by ID:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Lấy lịch sử hiến máu theo User ID (của người hiến).
 * @param {string} donorUserId
 * @returns {Promise<Array<object>>} Danh sách DonationHistoryDetailDto
 */
export const getDonationHistoryByDonorUserId = async (donorUserId) => {
    try {
        // Assume you have an endpoint like /DonationHistory/by-donor/{donorUserId}
        // If not, you might need to adjust your backend or filter on the client side
        const response = await api.get(`/DonationHistory/by-donor/${donorUserId}`);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể lấy lịch sử hiến máu của người dùng.';
        console.error('Error fetching donation history by donor user ID:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Tạo một bản ghi lịch sử hiến máu mới.
 * Yêu cầu Admin hoặc Staff role.
 * @param {object} createDto
 * @returns {Promise<object>} DonationHistoryDto đã tạo
 */
export const createDonationHistory = async (createDto) => {
    try {
        const response = await api.post('/DonationHistory', createDto);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể tạo lịch sử hiến máu.';
        console.error('Error creating donation history:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Cập nhật một bản ghi lịch sử hiến máu.
 * Yêu cầu Admin hoặc Staff role.
 * @param {string} donationId
 * @param {object} updateDto
 * @returns {Promise<object>} DonationHistoryDto đã cập nhật
 */
export const updateDonationHistory = async (donationId, updateDto) => {
    try {
        const response = await api.put(`/DonationHistory/${donationId}`, updateDto);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể cập nhật lịch sử hiến máu.';
        console.error('Error updating donation history:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Xóa một bản ghi lịch sử hiến máu.
 * Yêu cầu Admin hoặc Staff role.
 * @param {string} donationId
 * @returns {Promise<void>}
 */
export const deleteDonationHistory = async (donationId) => {
    try {
        await api.delete(`/DonationHistory/${donationId}`);
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể xóa lịch sử hiến máu.';
        console.error('Error deleting donation history:', errorMessage);
        throw new Error(errorMessage);
    }
};

// --- DONATION REQUEST API CALLS ---

/**
 * Đăng ký yêu cầu hiến máu.
 * @param {object} requestDto - DonationRequestInputDto
 * @returns {Promise<object>} DonationRequestDto đã tạo
 */
export const registerDonationRequest = async (requestDto) => {
    try {
        const response = await api.post('/DonationRequest/RegisterDonationRequest', requestDto);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Đăng ký yêu cầu hiến máu thất bại.';
        console.error('Error registering donation request:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Lấy tất cả các yêu cầu hiến máu.
 * Yêu cầu Admin hoặc Staff role.
 * @returns {Promise<Array<object>>} Danh sách DonationRequestDto
 */
export const getAllDonationRequests = async () => {
    try {
        const response = await api.get('/DonationRequest'); // Endpoint GET All Donations
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể lấy tất cả yêu cầu hiến máu.';
        console.error('Error fetching all donation requests:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Lấy yêu cầu hiến máu theo ID.
 * Yêu cầu Admin hoặc Staff role.
 * @param {string} requestId
 * @returns {Promise<object>} DonationRequestDto
 */
export const getDonationRequestById = async (requestId) => {
    try {
        const response = await api.get(`/DonationRequest/${requestId}`);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không tìm thấy yêu cầu hiến máu.';
        console.error('Error fetching donation request by ID:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Cập nhật yêu cầu hiến máu.
 * Yêu cầu Admin hoặc Staff role.
 * @param {string} requestId
 * @param {object} updateDto
 * @returns {Promise<object>} DonationRequestDto đã cập nhật
 */
export const updateDonationRequest = async (requestId, updateDto) => {
    try {
        const response = await api.put(`/DonationRequest/${requestId}`, updateDto);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể cập nhật yêu cầu hiến máu.';
        console.error('Error updating donation request:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Xóa yêu cầu hiến máu.
 * Yêu cầu Admin hoặc Staff role.
 * @param {string} requestId
 * @returns {Promise<void>}
 */
export const deleteDonationRequest = async (requestId) => {
    try {
        await api.delete(`/DonationRequest/${requestId}`);
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể xóa yêu cầu hiến máu.';
        console.error('Error deleting donation request:', errorMessage);
        throw new Error(errorMessage);
    }
};