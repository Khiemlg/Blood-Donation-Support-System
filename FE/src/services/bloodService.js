// src/services/bloodService.js
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

// --- BLOOD UNIT API CALLS ---

/**
 * Lấy tất cả các đơn vị máu.
 * @returns {Promise<Array<object>>} Danh sách BloodUnitInventoryDto
 */
export const getAllBloodUnits = async () => {
    try {
        const response = await api.get('/BloodUnit');
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể lấy tất cả đơn vị máu.';
        console.error('Error fetching all blood units:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Lấy đơn vị máu theo ID.
 * @param {string} unitId
 * @returns {Promise<object>} BloodUnitInventoryDto
 */
export const getBloodUnitById = async (unitId) => {
    try {
        const response = await api.get(`/BloodUnit/${unitId}`);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không tìm thấy đơn vị máu.';
        console.error('Error fetching blood unit by ID:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Lấy các đơn vị máu theo loại máu.
 * @param {number} bloodTypeId
 * @returns {Promise<Array<object>>} Danh sách BloodUnitInventoryDto
 */
export const getBloodUnitsByBloodTypeId = async (bloodTypeId) => {
    try {
        const response = await api.get(`/BloodUnit/bybloodtype/${bloodTypeId}`);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể lấy đơn vị máu theo loại.';
        console.error('Error fetching blood units by blood type:', errorMessage);
        throw new Error(errorMessage);
    }
};

// ... Thêm các hàm POST, PUT, DELETE cho BloodUnit nếu có trong backend của bạn

// --- BLOOD COMPONENT API CALLS ---

/**
 * Lấy tất cả các thành phần máu (hồng cầu, huyết tương, v.v.).
 * @returns {Promise<Array<object>>} Danh sách BloodComponentDto
 */
export const getAllBloodComponents = async () => {
    try {
        const response = await api.get('/BloodComponent');
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể lấy tất cả thành phần máu.';
        console.error('Error fetching all blood components:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Lấy thành phần máu theo ID.
 * @param {number} componentId
 * @returns {Promise<object>} BloodComponentDto
 */
export const getBloodComponentById = async (componentId) => {
    try {
        const response = await api.get(`/BloodComponent/${componentId}`);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không tìm thấy thành phần máu.';
        console.error('Error fetching blood component by ID:', errorMessage);
        throw new Error(errorMessage);
    }
};

// ... Thêm các hàm POST, PUT, DELETE cho BloodComponent nếu có trong backend của bạn

// --- BLOOD TYPE API CALLS ---

/**
 * Lấy tất cả các loại nhóm máu (A+, B-, v.v.).
 * @returns {Promise<Array<object>>} Danh sách BloodTypeDto
 */
export const getAllBloodTypes = async () => {
    try {
        const response = await api.get('/BloodType');
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không thể lấy tất cả loại máu.';
        console.error('Error fetching all blood types:', errorMessage);
        throw new Error(errorMessage);
    }
};

/**
 * Lấy loại máu theo ID.
 * @param {number} bloodTypeId
 * @returns {Promise<object>} BloodTypeDto
 */
export const getBloodTypeById = async (bloodTypeId) => {
    try {
        const response = await api.get(`/BloodType/${bloodTypeId}`);
        return response.data;
    } catch (error) {
        const errorMessage = error.response?.data?.message || 'Không tìm thấy loại máu.';
        console.error('Error fetching blood type by ID:', errorMessage);
        throw new Error(errorMessage);
    }
};

// ... Thêm các hàm POST, PUT, DELETE cho BloodType nếu có trong backend của bạn