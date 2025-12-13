import apiClient from '@/lib/axios';
import { ApiResponse, Movie, PaginatedResponse, SearchFilters, PaginationParams } from '@/types';

export const movieService = {
  // Get all movies with filters
  async getMovies(
    filters?: SearchFilters,
    pagination?: PaginationParams
  ): Promise<ApiResponse<PaginatedResponse<Movie>>> {
    const params = { ...filters, ...pagination };
    const response = await apiClient.get('/movies', { params });
    return response.data;
  },

  // Get movie by ID
  async getMovieById(id: string): Promise<ApiResponse<Movie>> {
    const response = await apiClient.get(`/movies/${id}`);
    return response.data;
  },

  // Get trending movies
  async getTrendingMovies(): Promise<ApiResponse<Movie[]>> {
    const response = await apiClient.get('/movies/trending');
    return response.data;
  },

  // Get popular movies
  async getPopularMovies(): Promise<ApiResponse<Movie[]>> {
    const response = await apiClient.get('/movies/popular');
    return response.data;
  },

  // Get top rated movies
  async getTopRatedMovies(): Promise<ApiResponse<Movie[]>> {
    const response = await apiClient.get('/movies/top-rated');
    return response.data;
  },

  // Get movies by genre
  async getMoviesByGenre(genre: string): Promise<ApiResponse<Movie[]>> {
    const response = await apiClient.get(`/movies/genre/${genre}`);
    return response.data;
  },

  // Get related movies
  async getRelatedMovies(movieId: string): Promise<ApiResponse<Movie[]>> {
    const response = await apiClient.get(`/movies/${movieId}/related`);
    return response.data;
  },

  // Search movies
  async searchMovies(query: string): Promise<ApiResponse<Movie[]>> {
    const response = await apiClient.get('/movies/search', { params: { q: query } });
    return response.data;
  },

  // Get all genres
  async getGenres(): Promise<ApiResponse<string[]>> {
    const response = await apiClient.get('/movies/genres');
    return response.data;
  },
};
