import apiClient from '@/lib/api';
import { ApiResponse, TorrentInfo, TorrentStats } from '@/types';

export const torrentService = {
  // Start seeding
  async startSeeding(movieId: string): Promise<ApiResponse<TorrentInfo>> {
    const response = await apiClient.post('/api/torrent/seed', { movieId });
    return response.data;
  },

  // Stop seeding
  async stopSeeding(movieId: string): Promise<ApiResponse<null>> {
    const response = await apiClient.delete(`/api/torrent/seed/${movieId}`);
    return response.data;
  },

  // Get torrent info
  async getTorrentInfo(movieId: string): Promise<ApiResponse<TorrentInfo>> {
    const response = await apiClient.get(`/api/torrent/${movieId}`);
    return response.data;
  },

  // Get all active seeds
  async getActiveSeeds(): Promise<ApiResponse<TorrentInfo[]>> {
    const response = await apiClient.get('/api/torrent/seeds');
    return response.data;
  },

  // Get torrent stats
  async getTorrentStats(): Promise<ApiResponse<TorrentStats>> {
    const response = await apiClient.get('/api/torrent/stats');
    return response.data;
  },

  // Get magnet link
  async getMagnetLink(movieId: string): Promise<ApiResponse<{ magnetURI: string }>> {
    const response = await apiClient.get(`/api/torrent/${movieId}/magnet`);
    return response.data;
  },
};
