'use client';

import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from './redux';
import {
  setMovies,
  setCurrentMovie,
  setTrending,
  setPopular,
  setTopRated,
  setRelatedMovies,
  setGenres,
  setLoading,
  setError,
} from '@/store/slices/movieSlice';
import { movieService } from '@/services/movie.service';
import { SearchFilters, PaginationParams } from '@/types';

export function useMovies() {
  const dispatch = useAppDispatch();
  const movieState = useAppSelector((state) => state.movie);

  const fetchMovies = async (filters?: SearchFilters, pagination?: PaginationParams) => {
    try {
      dispatch(setLoading(true));
      const response = await movieService.getMovies(filters, pagination);
      if (response.success && response.data) {
        dispatch(setMovies(response.data.data));
      }
      dispatch(setLoading(false));
    } catch (error: any) {
      dispatch(setError(error.message));
      dispatch(setLoading(false));
    }
  };

  const fetchMovieById = async (id: string) => {
    try {
      dispatch(setLoading(true));
      const response = await movieService.getMovieById(id);
      if (response.success && response.data) {
        dispatch(setCurrentMovie(response.data));
      }
      dispatch(setLoading(false));
    } catch (error: any) {
      dispatch(setError(error.message));
      dispatch(setLoading(false));
    }
  };

  // NOTE: Backend endpoint not yet implemented - temporarily disabled
  // const fetchTrending = async () => {
  //   try {
  //     dispatch(setLoading(true));
  //     const response = await movieService.getTrendingMovies();
  //     if (response.success && response.data) {
  //       dispatch(setTrending(response.data));
  //     }
  //     dispatch(setLoading(false));
  //   } catch (error: any) {
  //     dispatch(setError(error.message));
  //     dispatch(setLoading(false));
  //   }
  // };

  // NOTE: Backend endpoint not yet implemented - temporarily disabled
  // const fetchPopular = async () => {
  //   try {
  //     dispatch(setLoading(true));
  //     const response = await movieService.getPopularMovies();
  //     if (response.success && response.data) {
  //       dispatch(setPopular(response.data));
  //     }
  //     dispatch(setLoading(false));
  //   } catch (error: any) {
  //     dispatch(setError(error.message));
  //     dispatch(setLoading(false));
  //   }
  // };

  // NOTE: Backend endpoint not yet implemented - temporarily disabled
  // const fetchTopRated = async () => {
  //   try {
  //     dispatch(setLoading(true));
  //     const response = await movieService.getTopRatedMovies();
  //     if (response.success && response.data) {
  //       dispatch(setTopRated(response.data));
  //     }
  //     dispatch(setLoading(false));
  //   } catch (error: any) {
  //     dispatch(setError(error.message));
  //     dispatch(setLoading(false));
  //   }
  // };

  const fetchRelatedMovies = async (movieId: string) => {
    try {
      dispatch(setLoading(true));
      const response = await movieService.getRelatedMovies(movieId);
      if (response.success && response.data) {
        dispatch(setRelatedMovies(response.data));
      }
      dispatch(setLoading(false));
    } catch (error: any) {
      dispatch(setError(error.message));
      dispatch(setLoading(false));
    }
  };

  const fetchGenres = async () => {
    try {
      const response = await movieService.getGenres();
      if (response.success && response.data) {
        dispatch(setGenres(response.data));
      }
    } catch (error: any) {
      dispatch(setError(error.message));
    }
  };

  const searchMovies = async (query: string) => {
    try {
      dispatch(setLoading(true));
      const response = await movieService.searchMovies(query);
      if (response.success && response.data) {
        dispatch(setMovies(response.data));
      }
      dispatch(setLoading(false));
    } catch (error: any) {
      dispatch(setError(error.message));
      dispatch(setLoading(false));
    }
  };

  return {
    movies: movieState.movies,
    currentMovie: movieState.currentMovie,
    trending: movieState.trending,
    popular: movieState.popular,
    topRated: movieState.topRated,
    relatedMovies: movieState.relatedMovies,
    genres: movieState.genres,
    isLoading: movieState.isLoading,
    error: movieState.error,
    fetchMovies,
    fetchMovieById,
    // fetchTrending, // Disabled - endpoint not implemented
    // fetchPopular, // Disabled - endpoint not implemented
    // fetchTopRated, // Disabled - endpoint not implemented
    fetchRelatedMovies,
    fetchGenres,
    searchMovies,
  };
}
