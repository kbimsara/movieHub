import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import userReducer from './slices/userSlice';
import movieReducer from './slices/movieSlice';
import searchReducer from './slices/searchSlice';
import libraryReducer from './slices/librarySlice';
import playerReducer from './slices/playerSlice';
import torrentReducer from './slices/torrentSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    user: userReducer,
    movie: movieReducer,
    search: searchReducer,
    library: libraryReducer,
    player: playerReducer,
    torrent: torrentReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false,
    }),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
