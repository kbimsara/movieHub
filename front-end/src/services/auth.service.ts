import apiClient from '@/lib/axios';
import { ApiResponse, LoginCredentials, RegisterData, User } from '@/types';

export const authService = {
  // Login
  async login(credentials: LoginCredentials): Promise<ApiResponse<{ user: User; accessToken: string; refreshToken: string }>> {
    const response = await apiClient.post('/auth/login', credentials);
    return response.data;
  },

  // Register
  async register(data: RegisterData): Promise<ApiResponse<{ user: User; accessToken: string; refreshToken: string }>> {
    const response = await apiClient.post('/auth/register', data);
    return response.data;
  },

  // Logout
  async logout(): Promise<ApiResponse<null>> {
    const response = await apiClient.post('/auth/logout');
    return response.data;
  },

  // Forgot password
  async forgotPassword(email: string): Promise<ApiResponse<null>> {
    const response = await apiClient.post('/auth/forgot-password', { email });
    return response.data;
  },

  // Reset password
  async resetPassword(token: string, password: string): Promise<ApiResponse<null>> {
    const response = await apiClient.post('/auth/reset-password', { token, password });
    return response.data;
  },

  // Get current user
  async getCurrentUser(): Promise<ApiResponse<User>> {
    const response = await apiClient.get('/auth/me');
    return response.data;
  },

  // Refresh token
  async refreshToken(refreshToken: string): Promise<ApiResponse<{ accessToken: string }>> {
    const response = await apiClient.post('/auth/refresh', { refreshToken });
    return response.data;
  },
};
