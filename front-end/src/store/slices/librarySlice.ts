import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { LibraryItem } from '@/types';

interface LibraryState {
  items: LibraryItem[];
  continueWatching: LibraryItem[];
  favorites: LibraryItem[];
  isLoading: boolean;
  error: string | null;
}

const initialState: LibraryState = {
  items: [],
  continueWatching: [],
  favorites: [],
  isLoading: false,
  error: null,
};

const librarySlice = createSlice({
  name: 'library',
  initialState,
  reducers: {
    setLibrary: (state, action: PayloadAction<LibraryItem[]>) => {
      state.items = action.payload;
      state.error = null;
    },
    setContinueWatching: (state, action: PayloadAction<LibraryItem[]>) => {
      state.continueWatching = action.payload;
    },
    setFavorites: (state, action: PayloadAction<LibraryItem[]>) => {
      state.favorites = action.payload;
    },
    addToLibrary: (state, action: PayloadAction<LibraryItem>) => {
      state.items.push(action.payload);
    },
    removeFromLibrary: (state, action: PayloadAction<string>) => {
      state.items = state.items.filter(item => item.movieId !== action.payload);
      state.continueWatching = state.continueWatching.filter(item => item.movieId !== action.payload);
      state.favorites = state.favorites.filter(item => item.movieId !== action.payload);
    },
    updateLibraryItem: (state, action: PayloadAction<LibraryItem>) => {
      const index = state.items.findIndex(item => item.id === action.payload.id);
      if (index !== -1) {
        state.items[index] = action.payload;
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
  setLibrary,
  setContinueWatching,
  setFavorites,
  addToLibrary,
  removeFromLibrary,
  updateLibraryItem,
  setLoading,
  setError,
} = librarySlice.actions;

export default librarySlice.reducer;
