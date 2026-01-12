import apiClient from '@/lib/api';
import { ApiResponse, MovieMetadata, UploadProgress } from '@/types';
import { fileService, FileUploadResponse } from './file.service';

export const uploadService = {
  // Upload movie file with poster and metadata
  async uploadMovie(
    file: File,
    metadata: MovieMetadata,
    onProgress?: (progress: number) => void
  ): Promise<ApiResponse<{ uploadId: string; movieId: string }>> {
    const formData = new FormData();
    formData.append('file', file);
    
    // Add poster if provided
    if (metadata.poster) {
      formData.append('poster', metadata.poster);
    }
    
    // Add metadata as JSON
    formData.append('metadata', JSON.stringify({
      title: metadata.title,
      description: metadata.description,
      year: metadata.year,
      duration: metadata.duration,
      genres: metadata.genres,
      quality: metadata.quality,
      rating: metadata.rating,
      tags: metadata.tags,
      cast: metadata.cast,
      director: metadata.director,
      trailer: metadata.trailer,
    }));

    const response = await apiClient.post('/api/upload', formData, {
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
    const response = await apiClient.get(`/api/upload/${uploadId}/status`);
    return response.data;
  },

  // Cancel upload
  async cancelUpload(uploadId: string): Promise<ApiResponse<null>> {
    const response = await apiClient.delete(`/api/upload/${uploadId}`);
    return response.data;
  },

  // Get all uploads
  async getUploads(): Promise<ApiResponse<UploadProgress[]>> {
    const response = await apiClient.get('/api/upload');
    return response.data;
  },
};
