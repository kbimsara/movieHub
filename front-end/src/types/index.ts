// User and Authentication Types
export interface User {
  id: string;
  email: string;
  username: string;
  avatar?: string;
  role: 'user' | 'admin';
  createdAt: string;
}

export interface AuthState {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface RegisterData {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

// Movie Types
export interface Movie {
  id: string;
  title: string;
  description: string;
  poster: string;
  backdrop?: string;
  trailer?: string;
  year: number;
  duration: number; // in minutes
  genres?: string[];
  tags?: string[];
  rating: number;
  quality: '480p' | '720p' | '1080p' | '4K';
  cast?: CastMember[];
  director?: string;
  streamUrl?: string;
  downloadUrl?: string;
  torrentMagnet?: string;
  subtitles?: Subtitle[];
  createdAt: string;
  views: number;
}

export interface CastMember {
  id: string;
  name: string;
  character: string;
  photo?: string;
}

export interface Subtitle {
  language: string;
  url: string;
  label: string;
}

// Library Types
export interface LibraryItem {
  id: string;
  movieId: string;
  movie: Movie;
  userId: string;
  progress: number; // percentage 0-100
  lastWatched: string;
  isFavorite: boolean;
  addedAt: string;
}

export interface WatchHistory {
  id: string;
  movieId: string;
  movie: Movie;
  userId: string;
  watchedAt: string;
  progress: number;
  completed: boolean;
}

// Upload Types
export interface UploadProgress {
  id: string;
  fileName: string;
  fileSize: number;
  progress: number; // 0-100
  status: 'uploading' | 'processing' | 'ready' | 'failed';
  error?: string;
  movieId?: string;
}

export interface MovieMetadata {
  title: string;
  description: string;
  year: number;
  duration: number;
  genres: string[];
  quality: '480p' | '720p' | '1080p' | '4K';
  rating?: number;
  tags: string[];
  cast: string[];
  director?: string;
  trailer?: string;
  poster?: File;
}

// File Management Types
export interface FileMetadata {
  id: string;
  fileName: string;
  originalName: string;
  fileSize: number;
  mimeType: string;
  fileType: 'video' | 'image' | 'subtitle' | 'other';
  url: string;
  path: string;
  uploadedAt: string;
  userId: string;
  movieId?: string;
  width?: number;
  height?: number;
  duration?: number;
  thumbnailUrl?: string;
}

export interface FileUploadResponse {
  fileId: string;
  url: string;
  path: string;
  metadata: FileMetadata;
}

export interface StorageStats {
  totalFiles: number;
  totalSize: number;
  videoCount: number;
  imageCount: number;
  subtitleCount: number;
  userQuota: number;
  usedQuota: number;
}

// Torrent Types
export interface TorrentInfo {
  id: string;
  movieId: string;
  magnetURI: string;
  infoHash: string;
  isSeeding: boolean;
  uploadSpeed: number; // bytes per second
  downloadSpeed: number;
  uploaded: number; // total bytes
  downloaded: number;
  peers: number;
  progress: number; // 0-100
  ratio: number;
}

export interface TorrentStats {
  totalSeeds: number;
  totalUploaded: number;
  totalDownloaded: number;
  activeSeeds: number;
}

// Video Player Types
export interface PlayerState {
  isPlaying: boolean;
  currentTime: number;
  duration: number;
  volume: number;
  isMuted: boolean;
  quality: string;
  selectedSubtitle?: string;
  isFullscreen: boolean;
}

export interface VideoQuality {
  label: string;
  url: string;
  resolution: string;
}

// Search and Filter Types
export interface SearchFilters {
  query?: string;
  genres?: string[];
  year?: number;
  quality?: string[];
  rating?: number;
  sortBy?: 'title' | 'year' | 'rating' | 'views' | 'createdAt' | 'relevance';
  sortOrder?: 'asc' | 'desc';
}

export interface PaginationParams {
  page: number;
  limit: number;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}

// API Response Types
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  error?: string;
}

export interface ApiError {
  message: string;
  statusCode: number;
  errors?: Record<string, string[]>;
}
