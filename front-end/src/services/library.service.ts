import apiClient from '@/lib/axios';
import { ApiResponse, LibraryItem, WatchHistory, PaginatedResponse } from '@/types';

export const libraryService = {
  // Get user library
  async getLibrary(): Promise<ApiResponse<LibraryItem[]>> {
    const response = await apiClient.get('/library');
    return response.data;
  },

  // Add movie to library
  async addToLibrary(movieId: string): Promise<ApiResponse<LibraryItem>> {
    const response = await apiClient.post('/library', { movieId });
    return response.data;
  },

  // Remove from library
  async removeFromLibrary(movieId: string): Promise<ApiResponse<null>> {
    const response = await apiClient.delete(`/library/${movieId}`);
    return response.data;
  },

  // Toggle favorite
  async toggleFavorite(movieId: string): Promise<ApiResponse<LibraryItem>> {
    const response = await apiClient.put(`/library/${movieId}/favorite`);
    return response.data;
  },

  // Update watch progress
  async updateProgress(movieId: string, progress: number): Promise<ApiResponse<LibraryItem>> {
    const response = await apiClient.put(`/library/${movieId}/progress`, { progress });
    return response.data;
  },

  // Get continue watching
  async getContinueWatching(): Promise<ApiResponse<LibraryItem[]>> {
    const response = await apiClient.get('/library/continue-watching');
    return response.data;
  },

  // Get favorites
  async getFavorites(): Promise<ApiResponse<LibraryItem[]>> {
    const response = await apiClient.get('/library/favorites');
    return response.data;
  },

  // Get watch history
  async getWatchHistory(page: number = 1, limit: number = 20): Promise<ApiResponse<PaginatedResponse<WatchHistory>>> {
    const response = await apiClient.get('/library/history', { params: { page, limit } });
    return response.data;
  },

  // Clear watch history
  async clearHistory(): Promise<ApiResponse<null>> {
    const response = await apiClient.delete('/library/history');
    return response.data;
  },
};
