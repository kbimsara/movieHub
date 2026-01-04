import apiClient from '@/lib/api';
import { ApiResponse, LoginCredentials, RegisterData, User } from '@/types';

export const authService = {
  // Login
  async login(credentials: LoginCredentials): Promise<{ accessToken: string; refreshToken: string; expiresAt: string; user: { id: string; username: string; email: string } }> {
    const response = await apiClient.post('/api/auth/login', credentials);
    return response.data;
  },

  // Register
  async register(data: RegisterData): Promise<{ accessToken: string; refreshToken: string; expiresAt: string; user: { id: string; username: string; email: string } }> {
    const response = await apiClient.post('/api/auth/register', {
      username: data.username,
      email: data.email,
      password: data.password
    });
    return response.data;
  },

  // Logout
  async logout(refreshToken: string): Promise<ApiResponse<{ message: string }>> {
    const response = await apiClient.post('/api/auth/logout', { refreshToken });
    return response.data;
  },

  // Validate token
  async validateToken(): Promise<ApiResponse<{ valid: boolean; userId: string; email: string; username: string }>> {
    const response = await apiClient.get('/api/auth/validate');
    return response.data;
  },

  // Forgot password (not yet implemented in backend)
  // async forgotPassword(email: string): Promise<ApiResponse<null>> {
  //   const response = await apiClient.post('/api/auth/forgot-password', { email });
  //   return response.data;
  // },

  // Reset password (not yet implemented in backend)
  // async resetPassword(token: string, password: string): Promise<ApiResponse<null>> {
  //   const response = await apiClient.post('/api/auth/reset-password', { token, password });
  //   return response.data;
  // },

  // Get current user
  async getCurrentUser(): Promise<ApiResponse<User>> {
    const response = await apiClient.get('/api/auth/me');
    return response.data;
  },

  // Refresh token
  async refreshToken(refreshToken: string): Promise<ApiResponse<{ accessToken: string; refreshToken?: string }>> {
    const response = await apiClient.post('/api/auth/refresh', { refreshToken });
    return response.data;
  },
};
