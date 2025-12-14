import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Movie } from '@/types';

interface MovieState {
  movies: Movie[];
  currentMovie: Movie | null;
  trending: Movie[];
  popular: Movie[];
  topRated: Movie[];
  relatedMovies: Movie[];
  genres: string[];
  isLoading: boolean;
  error: string | null;
}

const initialState: MovieState = {
  movies: [],
  currentMovie: null,
  trending: [],
  popular: [],
  topRated: [],
  relatedMovies: [],
  genres: [],
  isLoading: false,
  error: null,
};

const movieSlice = createSlice({
  name: 'movie',
  initialState,
  reducers: {
    setMovies: (state, action: PayloadAction<Movie[]>) => {
      state.movies = action.payload;
      state.error = null;
    },
    setCurrentMovie: (state, action: PayloadAction<Movie | null>) => {
      state.currentMovie = action.payload;
      state.error = null;
    },
    setTrending: (state, action: PayloadAction<Movie[]>) => {
      state.trending = action.payload;
    },
    setPopular: (state, action: PayloadAction<Movie[]>) => {
      state.popular = action.payload;
    },
    setTopRated: (state, action: PayloadAction<Movie[]>) => {
      state.topRated = action.payload;
    },
    setRelatedMovies: (state, action: PayloadAction<Movie[]>) => {
      state.relatedMovies = action.payload;
    },
    setGenres: (state, action: PayloadAction<string[]>) => {
      state.genres = action.payload;
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.isLoading = action.payload;
    },
    setError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
      state.isLoading = false;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
});

export const {
  setMovies,
  setCurrentMovie,
  setTrending,
  setPopular,
  setTopRated,
  setRelatedMovies,
  setGenres,
  setLoading,
  setError,
  clearError,
} = movieSlice.actions;

export default movieSlice.reducer;
