import axios, { AxiosError, AxiosInstance, InternalAxiosRequestConfig } from 'axios';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5001/api/v1';

// Create axios instance
const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('accessToken');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// Response interceptor
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

    // Handle connection errors (service not available)
    if (error.code === 'ECONNREFUSED' || error.code === 'ERR_NETWORK' || !error.response) {
      console.warn(`Backend service not available: ${originalRequest.url}. Using fallback.`);
      return Promise.reject(error);
    }

    // Handle 404 errors (endpoint not implemented yet)
    if (error.response?.status === 404) {
      console.warn(`Endpoint not implemented: ${originalRequest.url}`);
      return Promise.reject(error);
    }

    // Handle 401 errors (token expired)
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
          throw new Error('No refresh token available');
        }

        // Try to refresh token
        const response = await axios.post(`${API_BASE_URL}/auth/refresh`, {
          RefreshToken: refreshToken,
        });

        const { data } = response.data;
        if (data?.accessToken) {
          localStorage.setItem('accessToken', data.accessToken);

          // Retry original request
          if (originalRequest.headers) {
            originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
          }
          return apiClient(originalRequest);
        }
        throw new Error('Invalid refresh response');
      } catch (refreshError) {
        // Refresh failed, clear tokens but don't redirect
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default apiClient;
