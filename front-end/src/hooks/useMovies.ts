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
import { getMockTrending, getMockPopular, getMockTopRated, mockMovies } from '@/lib/mockData';

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
      console.log('Backend not connected. Using mock data for movie details.');
      // Use mock data when API is not available
      const mockMovie = mockMovies.find(m => m.id === id);
      if (mockMovie) {
        dispatch(setCurrentMovie(mockMovie));
      }
      dispatch(setLoading(false));
    }
  };

  const fetchTrending = async () => {
    try {
      const response = await movieService.getTrendingMovies();
      if (response.success && response.data) {
        dispatch(setTrending(response.data));
      }
    } catch (error: any) {
      console.log('Backend not connected. Using mock data for trending movies.');
      // Use mock data when API is not available
      dispatch(setTrending(getMockTrending()));
    }
  };

  const fetchPopular = async () => {
    try {
      const response = await movieService.getPopularMovies();
      if (response.success && response.data) {
        dispatch(setPopular(response.data));
      }
    } catch (error: any) {
      console.log('Backend not connected. Using mock data for popular movies.');
      // Use mock data when API is not available
      dispatch(setPopular(getMockPopular()));
    }
  };

  const fetchTopRated = async () => {
    try {
      const response = await movieService.getTopRatedMovies();
      if (response.success && response.data) {
        dispatch(setTopRated(response.data));
      }
    } catch (error: any) {
      console.log('Backend not connected. Using mock data for top-rated movies.');
      // Use mock data when API is not available
      dispatch(setTopRated(getMockTopRated()));
    }
  };

  const fetchRelatedMovies = async (movieId: string) => {
    try {
      const response = await movieService.getRelatedMovies(movieId);
      if (response.success && response.data) {
        dispatch(setRelatedMovies(response.data));
      }
    } catch (error: any) {
      console.log('Backend not connected. Using mock data for related movies.');
      // Use mock data when API is not available - return random movies
      const currentIndex = mockMovies.findIndex(m => m.id === movieId);
      const otherMovies = mockMovies.filter(m => m.id !== movieId);
      const related = otherMovies.slice(0, 6);
      dispatch(setRelatedMovies(related));
    }
  };

  const fetchGenres = async () => {
    try {
      const response = await movieService.getGenres();
      if (response.success && response.data) {
        dispatch(setGenres(response.data));
      }
    } catch (error: any) {
      console.error('Error fetching genres:', error);
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
    fetchTrending,
    fetchPopular,
    fetchTopRated,
    fetchRelatedMovies,
    fetchGenres,
    searchMovies,
  };
}
