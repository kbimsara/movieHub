import apiClient from '@/lib/api';
import { ApiResponse, LibraryItem, WatchHistory, PaginatedResponse } from '@/types';

export const libraryService = {
  // Get user library
  async getLibrary(): Promise<ApiResponse<LibraryItem[]>> {
    const response = await apiClient.get('/api/library');
    return response.data;
  },

  // Add movie to library
  async addToLibrary(movieId: string): Promise<ApiResponse<LibraryItem>> {
    const response = await apiClient.post('/api/library', { movieId });
    return response.data;
  },

  // Remove from library
  async removeFromLibrary(movieId: string): Promise<ApiResponse<null>> {
    const response = await apiClient.delete(`/api/library/${movieId}`);
    return response.data;
  },

  // Toggle favorite
  async toggleFavorite(movieId: string): Promise<ApiResponse<LibraryItem>> {
    const response = await apiClient.put(`/api/library/${movieId}/favorite`);
    return response.data;
  },

  // Update watch progress
  async updateProgress(movieId: string, progress: number): Promise<ApiResponse<LibraryItem>> {
    const response = await apiClient.put(`/api/library/${movieId}/progress`, { progress });
    return response.data;
  },

  // NOTE: This endpoint is not yet implemented in the backend
  // Temporarily commented out to prevent 404 errors
  
  // Get continue watching
  // async getContinueWatching(): Promise<ApiResponse<LibraryItem[]>> {
  //   const response = await apiClient.get('/api/library/continue-watching');
  //   return response.data;
  // },

  // Get favorites
  async getFavorites(): Promise<ApiResponse<LibraryItem[]>> {
    const response = await apiClient.get('/api/library/favorites');
    return response.data;
  },

  // Get watch history
  async getWatchHistory(page: number = 1, limit: number = 20): Promise<ApiResponse<PaginatedResponse<WatchHistory>>> {
    const response = await apiClient.get('/api/library/history', { params: { page, limit } });
    return response.data;
  },

  // Clear watch history
  async clearHistory(): Promise<ApiResponse<null>> {
    const response = await apiClient.delete('/api/library/history');
    return response.data;
  },
};
