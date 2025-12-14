import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { TorrentInfo } from '@/types';

interface TorrentState {
  activeSeeds: TorrentInfo[];
  isLoading: boolean;
  error: string | null;
}

const initialState: TorrentState = {
  activeSeeds: [],
  isLoading: false,
  error: null,
};

const torrentSlice = createSlice({
  name: 'torrent',
  initialState,
  reducers: {
    setActiveSeeds: (state, action: PayloadAction<TorrentInfo[]>) => {
      state.activeSeeds = action.payload;
      state.error = null;
    },
    addSeed: (state, action: PayloadAction<TorrentInfo>) => {
      const exists = state.activeSeeds.find(seed => seed.movieId === action.payload.movieId);
      if (!exists) {
        state.activeSeeds.push(action.payload);
      }
    },
    removeSeed: (state, action: PayloadAction<string>) => {
      state.activeSeeds = state.activeSeeds.filter(seed => seed.movieId !== action.payload);
    },
    updateSeed: (state, action: PayloadAction<TorrentInfo>) => {
      const index = state.activeSeeds.findIndex(seed => seed.id === action.payload.id);
      if (index !== -1) {
        state.activeSeeds[index] = action.payload;
      }
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

export const {
  setActiveSeeds,
  addSeed,
  removeSeed,
  updateSeed,
  setLoading,
  setError,
} = torrentSlice.actions;

export default torrentSlice.reducer;
