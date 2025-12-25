import axios from 'axios';
import { ApiResponse, LibraryItem, WatchHistory, PaginatedResponse } from '@/types';

const libraryClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_LIBRARY_API_URL || 'http://localhost:5006/api/v1',
  timeout: 30000,
  headers: { 'Content-Type': 'application/json' },
});

export const libraryService = {
  // Get user library
  async getLibrary(): Promise<ApiResponse<LibraryItem[]>> {
    const response = await libraryClient.get('/Library');
    return response.data;
  },

  // Add movie to library
  async addToLibrary(movieId: string): Promise<ApiResponse<LibraryItem>> {
    const response = await libraryClient.post('/library', { movieId });
    return response.data;
  },

  // Remove from library
  async removeFromLibrary(movieId: string): Promise<ApiResponse<null>> {
    const response = await libraryClient.delete(`/library/${movieId}`);
    return response.data;
  },

  // Toggle favorite
  async toggleFavorite(movieId: string): Promise<ApiResponse<LibraryItem>> {
    const response = await libraryClient.put(`/library/${movieId}/favorite`);
    return response.data;
  },

  // Update watch progress
  async updateProgress(movieId: string, progress: number): Promise<ApiResponse<LibraryItem>> {
    const response = await libraryClient.put(`/library/${movieId}/progress`, { progress });
    return response.data;
  },

  // Get continue watching
  async getContinueWatching(): Promise<ApiResponse<LibraryItem[]>> {
    const response = await libraryClient.get('/Library/continue-watching');
    return response.data;
  },

  // Get favorites
  async getFavorites(): Promise<ApiResponse<LibraryItem[]>> {
    const response = await libraryClient.get('/library/favorites');
    return response.data;
  },

  // Get watch history
  async getWatchHistory(page: number = 1, limit: number = 20): Promise<ApiResponse<PaginatedResponse<WatchHistory>>> {
    const response = await libraryClient.get('/library/history', { params: { page, limit } });
    return response.data;
  },

  // Clear watch history
  async clearHistory(): Promise<ApiResponse<null>> {
    const response = await libraryClient.delete('/library/history');
    return response.data;
  },
};
