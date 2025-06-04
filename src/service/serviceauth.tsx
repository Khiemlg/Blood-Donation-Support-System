// src/services/api.ts
import axios from "axios";

// URL cơ sở cho API của bạn


/**
 * Gửi OTP đến email để đăng ký.
 * @param email Địa chỉ email của người dùng.
 */
export const registerSendOtp = (email: string) => {
  console.warn("Lưu ý: Endpoint C# `/api/User/Register` hiện không gửi OTP trực tiếp. Nếu bạn muốn luồng OTP, vui lòng đảm bảo các endpoint C# `Register` và `VerifyOtp` của bạn được kích hoạt và cấu hình.");
  return Promise.reject("Gửi OTP không được hỗ trợ trực tiếp bởi endpoint `/api/User/Register` hiện tại. Vui lòng xem lại cấu hình API C# của bạn.");
};

/**
 * Xác minh mã OTP cho việc đăng ký người dùng.
 * @param email Email của người dùng.
 * @param otpcode Mã OTP nhận được từ người dùng.
 */
export const verifyOtp = (email: string, otpcode: string) => {
  // Endpoint này đã bị chú thích trong mã C# của bạn.
  console.warn("Lưu ý: Endpoint C# `/api/Otp/verify-otp` hiện đang bị chú thích. Chức năng này sẽ không hoạt động trừ khi nó được kích hoạt trên backend.");
  return Promise.reject("Endpoint xác minh OTP không hoạt động trên backend.");
};

/**
 * Đăng ký người dùng mới.
 * @param username Tên người dùng đã chọn.
 * @param email Địa chỉ email của người dùng.
 * @param password Mật khẩu của người dùng (sẽ được hash ở backend).
 * @returns Một Promise với phản hồi từ axios.
 */
export const registerUser = (username: string, email: string, password: string) => {
    return axios.post(`https://localhost:7102/api/User/User/Register`, {
        Username: username,
        Email: email,
        PasswordHash: password // Tên trường phải khớp với DTO của bạn ở backend
    });
};

/**
 * Lấy danh sách tất cả người dùng.
 * @returns Một Promise với phản hồi từ axios chứa danh sách người dùng.
 */
export const getUsers = () => {
    return axios.get(`https://localhost:7102/api/User/User/List`); // Đảm bảo đúng endpoint
};

/**
 * Vô hiệu hóa người dùng (soft delete).
 * @param id The UserId of the user to deactivate.
 * @returns A promise with the axios response.
 */
export const deactivateUser = (id: string) => {
    return axios.post(`https://localhost:7102/api/User/User/Delete`, null, { params: { id } });
};

/**
 * Permanently deletes a user (hard delete).
 * @param id The UserId of the user to delete.
 * @returns A promise with the axios response.
 */
export const deleteUserPermanently = (id: string) => {
    return axios.post(`https://localhost:7102/api/User/User/Delete_2`, null, { params: { id } });
};

/**
 * Inserts a new user.
 * @param username The username.
 * @param roleId The role ID.
 * @param email The email address.
 * @param password The password.
 * @returns A promise with the axios response.
 */
export const insertUser = (username: string, roleId: number, email: string, password: string) => {
    return axios.post(`https://localhost:7102/api/User/User/Insert`, {
        Username: username,
        RoleID: roleId,
        Email: email,
        PasswordHash: password
    });
};

/**
 * Updates an existing user's information.
 * @param id The UserId of the user to update.
 * @param username The new username.
 * @param roleId The new role ID.
 * @param email The new email address.
 * @param phoneNumber The new phone number.
 * @param password The new password.
 * @returns A promise with the axios response.
 */
export const updateUser = (id: string, username: string, roleId: number, email: string, phoneNumber: string, password: string) => {
    return axios.post(`https://localhost:7102/api/User/User/Update`, {
        Username: username,
        RoleID: roleId,
        Email: email,
        PhoneNumber: phoneNumber,
        PasswordHash: password
    }, { params: { id } });
};

/**
 * Logs in a user.
 * @param email The user's email.
 * @param password The user's password.
 * @returns A promise with the axios response containing login details and token.
 */
export const loginUser = (email: string, password: string) => {
    return axios.post(`https://localhost:7102/api/User/User/Login`, {
        Email: email,
        Password: password
    });
};