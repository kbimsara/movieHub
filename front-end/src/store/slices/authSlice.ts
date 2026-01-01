import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { AuthState, User } from '@/types';
import { 
  saveSession, 
  getSession, 
  clearSession,
  updateAccessToken as updateStoredAccessToken,
  updateRefreshToken as updateStoredRefreshToken,
  updateUserData,
  extendSession
} from '@/utils/session';

// Don't restore session here to avoid hydration mismatch
// Session will be restored in useAuth hook after mount
const initialState: AuthState = {
  user: null,
  accessToken: null,
  refreshToken: null,
  isAuthenticated: false,
  isLoading: false,
  error: null,
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<{ user: User; accessToken: string; refreshToken: string; rememberMe?: boolean }>
    ) => {
      state.user = action.payload.user;
      state.accessToken = action.payload.accessToken;
      state.refreshToken = action.payload.refreshToken;
      state.isAuthenticated = true;
                  state.error = null;
            
                  // Save complete session data
                  saveSession(
                    action.payload.user,
                    action.payload.accessToken,
                    action.payload.refreshToken,
                    action.payload.rememberMe ?? true
                  );
                },    setUser: (state, action: PayloadAction<User>) => {
      state.user = action.payload;
      // Update user data in session storage
      updateUserData(action.payload);
    },
    updateTokens: (
      state,
      action: PayloadAction<{ accessToken: string; refreshToken?: string }>
    ) => {
      state.accessToken = action.payload.accessToken;
      if (action.payload.refreshToken) {
        state.refreshToken = action.payload.refreshToken;
      }
      
      // Update tokens in session storage
      updateStoredAccessToken(action.payload.accessToken);
      if (action.payload.refreshToken) {
        updateStoredRefreshToken(action.payload.refreshToken);
      }
      
      // Extend session expiry
      extendSession();
    },
    logout: (state) => {
      state.user = null;
      state.accessToken = null;
      state.refreshToken = null;
      state.isAuthenticated = false;
      state.error = null;
      
      // Clear all session data
      clearSession();
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.isLoading = action.payload;
    },
    setError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
      state.isLoading = false;
    },
  },
});

export const { setCredentials, setUser, updateTokens, logout, setLoading, setError } = authSlice.actions;
export default authSlice.reducer;
