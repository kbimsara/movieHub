import apiClient from '@/lib/api';
import { ApiResponse, Movie, PaginatedResponse, SearchFilters, PaginationParams } from '@/types';

export const movieService = {
  // Get all movies with filters
  async getMovies(
    filters?: SearchFilters,
    pagination?: PaginationParams
  ): Promise<ApiResponse<PaginatedResponse<Movie>>> {
    const params = { ...filters, ...pagination, includeUnpublished: true };
    const response = await apiClient.get('/api/movies', { params });
    return response.data;
  },

  // Get movie by ID
  async getMovieById(id: string): Promise<ApiResponse<Movie>> {
    const response = await apiClient.get(`/api/movies/${id}`);
    return response.data;
  },

  // Search movies (use search service for advanced search)
  async searchMovies(query: string): Promise<ApiResponse<Movie[]>> {
    const response = await apiClient.get('/api/search/movies', { params: { q: query } });
    return response.data;
  },

  // NOTE: Additional endpoints not yet implemented in catalog service.
  // Uncomment when backend endpoints are available:
  
  // async getTrendingMovies(): Promise<ApiResponse<Movie[]>> {
  //   const response = await apiClient.get('/api/movies/trending');
  //   return response.data;
  // },
  
  // async getPopularMovies(): Promise<ApiResponse<Movie[]>> {
  //   const response = await apiClient.get('/api/movies/popular');
  //   return response.data;
  // },
  
  // async getTopRatedMovies(): Promise<ApiResponse<Movie[]>> {
  //   const response = await apiClient.get('/api/movies/top-rated');
  //   return response.data;
  // },
  
  // async getMoviesByGenre(genre: string): Promise<ApiResponse<Movie[]>> {
  //   const response = await apiClient.get(`/api/movies/genre/${genre}`);
  //   return response.data;
  // },
  
  // async getRelatedMovies(movieId: string): Promise<ApiResponse<Movie[]>> {
  //   const response = await apiClient.get(`/api/movies/${movieId}/related`);
  //   return response.data;
  // },
  
  // async getGenres(): Promise<ApiResponse<string[]>> {
  //   const response = await apiClient.get('/api/movies/genres');
  //   return response.data;
  // },

  // Create movie (admin only)
  async createMovie(movie: Partial<Movie>): Promise<ApiResponse<Movie>> {
    const response = await apiClient.post('/api/movies', movie);
    return response.data;
  },

  // Update movie (admin only)
  async updateMovie(id: string, movie: Partial<Movie>): Promise<ApiResponse<Movie>> {
    const response = await apiClient.put(`/api/movies/${id}`, movie);
    return response.data;
  },

  // Delete movie (admin only)
  async deleteMovie(id: string): Promise<ApiResponse<null>> {
    const response = await apiClient.delete(`/api/movies/${id}`);
    return response.data;
  },
};
