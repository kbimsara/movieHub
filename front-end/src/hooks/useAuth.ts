'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAppDispatch, useAppSelector } from './redux';
import { setCredentials, logout as logoutAction, setUser } from '@/store/slices/authSlice';
import { clearUserData } from '@/store/slices/userSlice';
import { authService } from '@/services/auth.service';
import { LoginCredentials, RegisterData } from '@/types';

export function useAuth() {
  const dispatch = useAppDispatch();
  const router = useRouter();
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

  // Listen for logout events (from 401 responses in axios interceptor)
  useEffect(() => {
    const handleLogout = () => {
      dispatch(logoutAction());
      dispatch(clearUserData());
      router.push('/auth/login');
    };

    window.addEventListener('auth:logout', handleLogout);
    return () => window.removeEventListener('auth:logout', handleLogout);
  }, [dispatch, router]);

  const login = async (credentials: LoginCredentials) => {
    try {
      // Clear any existing tokens before login
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      
      const response = await authService.login(credentials);
      if (response.token && response.email) {
        // Store token
        localStorage.setItem('accessToken', response.token);
        
        // Create user object from email
        const user = {
          id: '', // Backend doesn't return ID yet
          email: response.email,
          username: response.email.split('@')[0],
          role: 'user' as const,
          createdAt: new Date().toISOString()
        };
        
        dispatch(setCredentials({
          user,
          accessToken: response.token,
          refreshToken: response.token // Using same token for now
        }));
        
        return { success: true };
      }
      return { success: false, error: 'Login failed' };
    } catch (error: any) {
      const errorMessage = error.response?.data?.error || error.message || 'Login failed';
      return { success: false, error: errorMessage };
    }
  };

  const register = async (data: RegisterData) => {
    try {
      const response = await authService.register(data);
      if (response.token && response.email) {
        // Store token
        localStorage.setItem('accessToken', response.token);
        
        // Create user object from email
        const user = {
          id: '', // Backend doesn't return ID yet
          email: response.email,
          username: response.email.split('@')[0],
          role: 'user' as const,
          createdAt: new Date().toISOString()
        };
        
        dispatch(setCredentials({
          user,
          accessToken: response.token,
          refreshToken: response.token // Using same token for now
        }));
        
        return { success: true };
      }
      return { success: false, error: 'Registration failed' };
    } catch (error: any) {
      const errorMessage = error.response?.data?.error || error.message || 'Registration failed';
      return { success: false, error: errorMessage };
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      dispatch(logoutAction());
      dispatch(clearUserData());
      router.push('/auth/login');
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
