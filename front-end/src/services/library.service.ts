import apiClient from '@/lib/api';
import { ApiResponse, LibraryItem, WatchHistory, Movie, PaginatedResponse } from '@/types';

/**
 * Library service
 *
 * All calls go to /api/library/* which the API Gateway rewrites to
 * /api/me/* on the user-service (MeController).
 */
export const libraryService = {
  // ── Library ──────────────────────────────────────────────────────────────

  async getLibrary(): Promise<ApiResponse<LibraryItem[]>> {
    const response = await apiClient.get('/api/library/library');
    return response.data;
  },

  async addToLibrary(movieId: string): Promise<ApiResponse<LibraryItem>> {
    const response = await apiClient.post('/api/library/library', { movieId });
    return response.data;
  },

  async removeFromLibrary(movieId: string): Promise<ApiResponse<null>> {
    const response = await apiClient.delete(`/api/library/library/${movieId}`);
    return response.data;
  },

  async toggleFavorite(movieId: string): Promise<ApiResponse<LibraryItem>> {
    const response = await apiClient.post(`/api/library/library/${movieId}/favorite`);
    return response.data;
  },

  async updateProgress(movieId: string, progress: number): Promise<ApiResponse<LibraryItem>> {
    const response = await apiClient.put(`/api/library/library/${movieId}/progress`, { progress });
    return response.data;
  },

  // ── Favourites ────────────────────────────────────────────────────────────

  async getFavorites(): Promise<ApiResponse<Movie[]>> {
    const response = await apiClient.get('/api/library/favorites');
    return response.data;
  },

  // ── Watch Later ───────────────────────────────────────────────────────────

  async getWatchLater(): Promise<ApiResponse<Movie[]>> {
    const response = await apiClient.get('/api/library/watch-later');
    return response.data;
  },

  async addToWatchLater(movieId: string): Promise<ApiResponse<null>> {
    const response = await apiClient.post('/api/library/watch-later', { movieId });
    return response.data;
  },

  async removeFromWatchLater(movieId: string): Promise<ApiResponse<null>> {
    const response = await apiClient.delete(`/api/library/watch-later/${movieId}`);
    return response.data;
  },

  // ── Watch History ─────────────────────────────────────────────────────────

  async getWatchHistory(): Promise<ApiResponse<WatchHistory[]>> {
    const response = await apiClient.get('/api/library/history');
    return response.data;
  },

  async addToWatchHistory(movieId: string, progress: number): Promise<ApiResponse<WatchHistory>> {
    const response = await apiClient.post('/api/library/history', { movieId, progress });
    return response.data;
  },
};
