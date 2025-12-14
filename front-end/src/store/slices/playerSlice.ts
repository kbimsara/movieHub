import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { PlayerState } from '@/types';

const initialState: PlayerState = {
  isPlaying: false,
  currentTime: 0,
  duration: 0,
  volume: 1,
  isMuted: false,
  quality: '720p',
  selectedSubtitle: undefined,
  isFullscreen: false,
};

const playerSlice = createSlice({
  name: 'player',
  initialState,
  reducers: {
    setPlaying: (state, action: PayloadAction<boolean>) => {
      state.isPlaying = action.payload;
    },
    setCurrentTime: (state, action: PayloadAction<number>) => {
      state.currentTime = action.payload;
    },
    setDuration: (state, action: PayloadAction<number>) => {
      state.duration = action.payload;
    },
    setVolume: (state, action: PayloadAction<number>) => {
      state.volume = action.payload;
    },
    setMuted: (state, action: PayloadAction<boolean>) => {
      state.isMuted = action.payload;
    },
    setQuality: (state, action: PayloadAction<string>) => {
      state.quality = action.payload;
    },
    setSubtitle: (state, action: PayloadAction<string | undefined>) => {
      state.selectedSubtitle = action.payload;
    },
    setFullscreen: (state, action: PayloadAction<boolean>) => {
      state.isFullscreen = action.payload;
    },
    resetPlayer: (state) => {
      return initialState;
    },
  },
});

export const {
  setPlaying,
  setCurrentTime,
  setDuration,
  setVolume,
  setMuted,
  setQuality,
  setSubtitle,
  setFullscreen,
  resetPlayer,
} = playerSlice.actions;

export default playerSlice.reducer;
