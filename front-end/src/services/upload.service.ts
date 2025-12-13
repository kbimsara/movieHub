import apiClient from '@/lib/axios';
import { ApiResponse, MovieMetadata, UploadProgress } from '@/types';

export const uploadService = {
  // Upload movie file
  async uploadMovie(
    file: File,
    metadata: MovieMetadata,
    onProgress?: (progress: number) => void
  ): Promise<ApiResponse<{ uploadId: string; movieId: string }>> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('metadata', JSON.stringify(metadata));

    const response = await apiClient.post('/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
      onUploadProgress: (progressEvent) => {
        if (progressEvent.total && onProgress) {
          const progress = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          onProgress(progress);
        }
      },
    });

    return response.data;
  },

  // Get upload status
  async getUploadStatus(uploadId: string): Promise<ApiResponse<UploadProgress>> {
    const response = await apiClient.get(`/upload/${uploadId}/status`);
    return response.data;
  },

  // Cancel upload
  async cancelUpload(uploadId: string): Promise<ApiResponse<null>> {
    const response = await apiClient.delete(`/upload/${uploadId}`);
    return response.data;
  },

  // Get all uploads
  async getUploads(): Promise<ApiResponse<UploadProgress[]>> {
    const response = await apiClient.get('/upload');
    return response.data;
  },
};
