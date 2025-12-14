import apiClient from '@/lib/axios';
import { ApiResponse, TorrentInfo, TorrentStats } from '@/types';

export const torrentService = {
  // Start seeding
  async startSeeding(movieId: string): Promise<ApiResponse<TorrentInfo>> {
    const response = await apiClient.post('/torrent/seed', { movieId });
    return response.data;
  },

  // Stop seeding
  async stopSeeding(movieId: string): Promise<ApiResponse<null>> {
    const response = await apiClient.delete(`/torrent/seed/${movieId}`);
    return response.data;
  },

  // Get torrent info
  async getTorrentInfo(movieId: string): Promise<ApiResponse<TorrentInfo>> {
    const response = await apiClient.get(`/torrent/${movieId}`);
    return response.data;
  },

  // Get all active seeds
  async getActiveSeeds(): Promise<ApiResponse<TorrentInfo[]>> {
    const response = await apiClient.get('/torrent/seeds');
    return response.data;
  },

  // Get torrent stats
  async getTorrentStats(): Promise<ApiResponse<TorrentStats>> {
    const response = await apiClient.get('/torrent/stats');
    return response.data;
  },

  // Get magnet link
  async getMagnetLink(movieId: string): Promise<ApiResponse<{ magnetURI: string }>> {
    const response = await apiClient.get(`/torrent/${movieId}/magnet`);
    return response.data;
  },
};
