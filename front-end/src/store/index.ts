import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import movieReducer from './slices/movieSlice';
import libraryReducer from './slices/librarySlice';
import playerReducer from './slices/playerSlice';
import torrentReducer from './slices/torrentSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    movie: movieReducer,
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
