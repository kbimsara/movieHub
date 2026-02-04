'use client';

import { useCallback, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAppDispatch, useAppSelector } from './redux';
import { authService } from '@/services/auth.service';
import { LoginCredentials, RegisterData, User } from '@/types';
import { decodeJWT, getSession, clearSession, isSessionValid } from '@/utils/session';
import {
  setCredentials,
  setUser,
  setLoading as setAuthLoading,
  setError as setAuthError,
  logout as logoutAction,
} from '@/store/slices/authSlice';
import { clearUserData } from '@/store/slices/userSlice';

export function useAuth() {
  const dispatch = useAppDispatch();
  const router = useRouter();
  const auth = useAppSelector((state) => state.auth);

  useEffect(() => {
    if (typeof window === 'undefined') {
      return;
    }

    if (auth.isAuthenticated) {
      return;
    }

    const session = getSession();
    if (!session) {
      return;
    }

    dispatch(
      setCredentials({
        user: session.user,
        accessToken: session.accessToken,
        refreshToken: session.refreshToken,
        rememberMe: true,
      })
    );
  }, [auth.isAuthenticated, dispatch]);

  useEffect(() => {
    if (!auth.isAuthenticated) {
      return;
    }

    if (isSessionValid()) {
      return;
    }

    clearSession();
    dispatch(logoutAction());
    dispatch(clearUserData());
  }, [auth.isAuthenticated, dispatch]);

  const login = useCallback(
    async (credentials: LoginCredentials, rememberMe: boolean = true) => {
      try {
        dispatch(setAuthLoading(true));
        dispatch(setAuthError(null));
        clearSession();

        const response = await authService.login(credentials);
        if (!response.token) {
          throw new Error('Login failed: missing access token');
        }

        const decoded = decodeJWT(response.token) || {};
        const fallbackUser: User = {
          id: decoded.sub ?? 'unknown',
          email: decoded.email ?? credentials.email,
          username: decoded.unique_name ?? credentials.email,
          role: decoded.role === 'admin' ? 'admin' : 'user',
          createdAt: decoded.createdAt ?? new Date().toISOString(),
        };

        dispatch(
          setCredentials({
            user: fallbackUser,
            accessToken: response.token,
            refreshToken: response.refreshToken,
            rememberMe,
          })
        );

        try {
          const profile = await authService.getCurrentUser();
          if (profile.success && profile.data) {
            dispatch(setUser(profile.data));
          }
        } catch (profileError) {
          console.warn('Unable to fetch profile after login:', profileError);
        }

        return { success: true };
      } catch (error: any) {
        const errorMessage = error?.response?.data?.error || error.message || 'Login failed';
        dispatch(setAuthError(errorMessage));
        return { success: false, error: errorMessage };
      } finally {
        dispatch(setAuthLoading(false));
      }
    },
    [dispatch]
  );

  const register = useCallback(
    async (data: RegisterData) => {
      try {
        dispatch(setAuthLoading(true));
        dispatch(setAuthError(null));

        const response = await authService.register(data);
        if (response.token) {
          router.push('/auth/login');
          return { success: true };
        }

        return { success: false, error: 'Registration failed' };
      } catch (error: any) {
        const errorMessage = error?.response?.data?.error || error.message || 'Registration failed';
        dispatch(setAuthError(errorMessage));
        return { success: false, error: errorMessage };
      } finally {
        dispatch(setAuthLoading(false));
      }
    },
    [dispatch, router]
  );

  const logout = useCallback(async () => {
    try {
      await authService.logout(auth.refreshToken ?? undefined);
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      clearSession();
      dispatch(logoutAction());
      dispatch(clearUserData());
      router.push('/auth/login');
    }
  }, [auth.refreshToken, dispatch, router]);

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
