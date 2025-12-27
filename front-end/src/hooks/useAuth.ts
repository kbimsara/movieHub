'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAppDispatch, useAppSelector } from './redux';
import { setCredentials, logout as logoutAction, setUser, updateTokens } from '@/store/slices/authSlice';
import { clearUserData } from '@/store/slices/userSlice';
import { authService } from '@/services/auth.service';
import { LoginCredentials, RegisterData } from '@/types';
import { getSession, isSessionValid, clearSession } from '@/utils/session';

export function useAuth() {
  const dispatch = useAppDispatch();
  const router = useRouter();
  const auth = useAppSelector((state) => state.auth);

  useEffect(() => {
    // Restore session from localStorage after mount (client-side only)
    const restoreSession = () => {
      const session = getSession();
      if (session && !auth.user) {
        // Restore user and tokens to Redux
        dispatch(setCredentials({
          user: session.user,
          accessToken: session.accessToken,
          refreshToken: session.refreshToken,
          rememberMe: true
        }));
      }
    };

    restoreSession();
  }, [dispatch, auth.user]);

  useEffect(() => {
    // Only validate session if we have a restored user and valid session
    if (!auth.user || !isSessionValid()) {
      return;
    }

    // Check if user data is fresh enough (skip validation if user was just restored)
    const checkAuth = async () => {
      try {
        const response = await authService.getCurrentUser();
        if (response.success && response.data) {
          // Update user data with fresh info from server
          dispatch(setUser(response.data));
        } else {
          // Invalid response, clear session
          clearSession();
          dispatch(logoutAction());
        }
      } catch (error) {
        console.error('Auth validation error:', error);
        // Only clear session if it's actually an auth error (401/403)
        // Don't clear on network errors or 404s
        if ((error as any)?.response?.status === 401 || (error as any)?.response?.status === 403) {
          clearSession();
          dispatch(logoutAction());
        }
      }
    };

    // Add a small delay to avoid calling this immediately after restore
    const timeoutId = setTimeout(checkAuth, 1000);
    return () => clearTimeout(timeoutId);
  }, []); // Only run once on mount after session restoration

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

  const login = async (credentials: LoginCredentials, rememberMe: boolean = true) => {
    try {
      // Clear any existing session before login
      clearSession();
      
      const response = await authService.login(credentials);
      if (response.token && response.email) {
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
          refreshToken: response.token, // Using same token for now
          rememberMe
        }));
        
        // Redirect to home page after successful login
        router.push('/');
        
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
          refreshToken: response.token, // Using same token for now
          rememberMe: true
        }));
        
        // Redirect to home page after successful registration
        router.push('/');
        
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
