'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAppDispatch, useAppSelector } from './redux';
import { setCredentials, logout as logoutAction, setUser, updateTokens, setLoading } from '@/store/slices/authSlice';
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
        console.log('ðŸ”„ Restoring session from localStorage');
        // Restore user and tokens to Redux
        dispatch(setCredentials({
          user: session.user,
          accessToken: session.accessToken,
          refreshToken: session.refreshToken,
          rememberMe: true
        }));
      } else if (!session && !auth.user) {
        // No session found, ensure loading is set to false
        dispatch(setLoading(false));
      }
    };

    restoreSession();
  }, [dispatch, auth.user]);

  useEffect(() => {
    // Only validate session if we have an authenticated user
    if (!auth.isAuthenticated || !auth.user) {
      return;
    }

    // Skip validation if session is not valid
    if (!isSessionValid()) {
      console.warn('âš ï¸ Session expired, logging out');
      clearSession();
      dispatch(logoutAction());
      return;
    }


    // NOTE: Backend validation is disabled because /api/auth/me endpoint
    // returns 401 even with valid tokens. The session is trusted based on
    // localStorage data until this endpoint is fixed on the backend.
    // 
    // When the backend /api/auth/me endpoint is fixed, uncomment this code:
    /*
    const checkAuth = async () => {
      try {
        console.log('🔍 Validating session with backend');
        const response = await authService.getCurrentUser();
        if (response.success && response.data) {
          console.log('✅ Session validated successfully');
          dispatch(setUser(response.data));
        }
      } catch (error) {
        console.error('❌ Auth validation error:', error);
        const status = (error as any)?.response?.status;
        if (status === 401 || status === 403) {
          clearSession();
          dispatch(logoutAction());
        }
      }
    };
    const timeoutId = setTimeout(checkAuth, 2000);
    return () => clearTimeout(timeoutId);
    */
  }, [auth.isAuthenticated, auth.user, dispatch]);

  // DISABLED: Automatic logout redirect removed
  // Users now stay logged in even if some API endpoints return 401
  // They can manually logout using the logout button
  /*
  useEffect(() => {
    const handleLogout = () => {
      dispatch(logoutAction());
      dispatch(clearUserData());
      router.push('/auth/login');
    };

    window.addEventListener('auth:logout', handleLogout);
    return () => window.removeEventListener('auth:logout', handleLogout);
  }, [dispatch, router]);
  */

  const login = async (credentials: LoginCredentials, rememberMe: boolean = true) => {
    try {
      // Clear any existing session before login
      clearSession();
      
      const response = await authService.login(credentials);
      if (response.accessToken && response.user) {
        // Use the user data from response
        const user = {
          id: response.user.id,
          email: response.user.email,
          username: response.user.username,
          role: 'user' as const,
          createdAt: new Date().toISOString()
        };
        
        dispatch(setCredentials({
          user,
          accessToken: response.accessToken,
          refreshToken: response.refreshToken,
          rememberMe
        }));
        
        // Don't navigate here - let the calling component handle navigation
        // This prevents race conditions with token storage
        
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
      if (response.accessToken && response.user) {
        // Use the user data from response
        const user = {
          id: response.user.id,
          email: response.user.email,
          username: response.user.username,
          role: 'user' as const,
          createdAt: new Date().toISOString()
        };
        
        dispatch(setCredentials({
          user,
          accessToken: response.accessToken,
          refreshToken: response.refreshToken,
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
