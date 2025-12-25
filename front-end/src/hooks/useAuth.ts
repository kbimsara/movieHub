'use client';

import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from './redux';
import { setCredentials, logout as logoutAction, setUser } from '@/store/slices/authSlice';
import { authService } from '@/services/auth.service';
import { LoginCredentials, RegisterData } from '@/types';

export function useAuth() {
  const dispatch = useAppDispatch();
  const auth = useAppSelector((state) => state.auth);

  useEffect(() => {
    // Check if user is already logged in
    const checkAuth = async () => {
      const accessToken = localStorage.getItem('accessToken');
      if (accessToken && !auth.user) {
        try {
          const response = await authService.getCurrentUser();
          if (response.success && response.data) {
            dispatch(setUser(response.data));
          } else {
            // Invalid response, clear tokens
            localStorage.removeItem('accessToken');
            localStorage.removeItem('refreshToken');
            dispatch(logoutAction());
          }
        } catch (error) {
          // Token expired or invalid, clear it silently
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          dispatch(logoutAction());
        }
      }
    };

    checkAuth();
  }, [dispatch, auth.user]);

  const login = async (credentials: LoginCredentials) => {
    try {
      // Clear any existing tokens before login
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      
      const response = await authService.login(credentials);
      if (response.success && response.data) {
        dispatch(setCredentials(response.data));
        return { success: true };
      }
      return { success: false, error: response.error || 'Login failed' };
    } catch (error: any) {
      return { success: false, error: error.message || 'Login failed' };
    }
  };

  const register = async (data: RegisterData) => {
    try {
      const response = await authService.register(data);
      if (response.success && response.data) {
        dispatch(setCredentials(response.data));
        return { success: true };
      }
      return { success: false, error: response.error || 'Registration failed' };
    } catch (error: any) {
      return { success: false, error: error.message || 'Registration failed' };
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      dispatch(logoutAction());
    }
  };

  return {
    user: auth.user,
    isAuthenticated: auth.isAuthenticated,
    isLoading: auth.isLoading,
    error: auth.error,
    login,
    register,
    logout,
  };
}
