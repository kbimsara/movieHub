import axios from 'axios';
import { ApiResponse, Movie, PaginatedResponse, SearchFilters, PaginationParams } from '@/types';

const movieClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_MOVIE_API_URL || 'http://localhost:5003/api/v1',
  timeout: 30000,
  headers: { 'Content-Type': 'application/json' },
});

export const movieService = {
  // Get all movies with filters
  async getMovies(
    filters?: SearchFilters,
    pagination?: PaginationParams
  ): Promise<ApiResponse<PaginatedResponse<Movie>>> {
    const params = { ...filters, ...pagination };
    const response = await movieClient.get('/Movies', { params });
    return response.data;
  },

  // Get movie by ID
  async getMovieById(id: string): Promise<ApiResponse<Movie>> {
    const response = await movieClient.get(`/Movies/${id}`);
    return response.data;
  },

  // Get trending movies
  async getTrendingMovies(): Promise<ApiResponse<Movie[]>> {
    const response = await movieClient.get('/Movies/trending');
    return response.data;
  },

  // Get popular movies
  async getPopularMovies(): Promise<ApiResponse<Movie[]>> {
    const response = await movieClient.get('/Movies/popular');
    return response.data;
  },

  // Get top rated movies
  async getTopRatedMovies(): Promise<ApiResponse<Movie[]>> {
    const response = await movieClient.get('/Movies/top-rated');
    return response.data;
  },

  // Get movies by genre
  async getMoviesByGenre(genre: string): Promise<ApiResponse<Movie[]>> {
    const response = await movieClient.get(`/movies/genre/${genre}`);
    return response.data;
  },

  // Get related movies
  async getRelatedMovies(movieId: string): Promise<ApiResponse<Movie[]>> {
    const response = await movieClient.get(`/movies/${movieId}/related`);
    return response.data;
  },

  // Search movies
  async searchMovies(query: string): Promise<ApiResponse<Movie[]>> {
    const response = await movieClient.get('/movies/search', { params: { q: query } });
    return response.data;
  },

  // Get all genres
  async getGenres(): Promise<ApiResponse<string[]>> {
    const response = await movieClient.get('/movies/genres');
    return response.data;
  },
};
