'use client';

import { useAppDispatch, useAppSelector } from './redux';
import {
  setLibrary,
  setContinueWatching,
  setFavorites,
  addToLibrary as addToLibraryAction,
  removeFromLibrary as removeFromLibraryAction,
  updateLibraryItem,
  setLoading,
  setError,
} from '@/store/slices/librarySlice';
import { libraryService } from '@/services/library.service';

export function useLibrary() {
  const dispatch = useAppDispatch();
  const libraryState = useAppSelector((state) => state.library);
  const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);

  const fetchLibrary = async () => {
    // Don't fetch library if user is not authenticated
    if (!isAuthenticated) {
      return;
    }
    try {
      dispatch(setLoading(true));
      const response = await libraryService.getLibrary();
      if (response.success && response.data) {
        dispatch(setLibrary(response.data));
      }
      dispatch(setLoading(false));
    } catch (error: any) {
      dispatch(setError(error.message));
    }
  };

  const addToLibrary = async (movieId: string) => {
    try {
      const response = await libraryService.addToLibrary(movieId);
      if (response.success && response.data) {
        dispatch(addToLibraryAction(response.data));
        return { success: true };
      }
      return { success: false };
    } catch (error: any) {
      return { success: false, error: error.message };
    }
  };

  const removeFromLibrary = async (movieId: string) => {
    try {
      await libraryService.removeFromLibrary(movieId);
      dispatch(removeFromLibraryAction(movieId));
      return { success: true };
    } catch (error: any) {
      return { success: false, error: error.message };
    }
  };

  const toggleFavorite = async (movieId: string) => {
    try {
      const response = await libraryService.toggleFavorite(movieId);
      if (response.success && response.data) {
        dispatch(updateLibraryItem(response.data));
        return { success: true };
      }
      return { success: false };
    } catch (error: any) {
      return { success: false, error: error.message };
    }
  };

  const updateProgress = async (movieId: string, progress: number) => {
    try {
      const response = await libraryService.updateProgress(movieId, progress);
      if (response.success && response.data) {
        dispatch(updateLibraryItem(response.data));
      }
    } catch (error: any) {
      console.error('Error updating progress:', error);
    }
  };

  // NOTE: Backend endpoint not yet implemented - temporarily disabled
  // const fetchContinueWatching = async () => {
  //   try {
  //     const response = await libraryService.getContinueWatching();
  //     if (response.success && response.data) {
  //       dispatch(setContinueWatching(response.data));
  //     }
  //   } catch (error: any) {
  //     console.error('Error fetching continue watching:', error);
  //   }
  // };

  const fetchFavorites = async () => {
    try {
      const response = await libraryService.getFavorites();
      if (response.success && response.data) {
        dispatch(setFavorites(response.data));
      }
    } catch (error: any) {
      console.error('Error fetching favorites:', error);
    }
  };

  return {
    library: libraryState.items,
    continueWatching: libraryState.continueWatching,
    favorites: libraryState.favorites,
    isLoading: libraryState.isLoading,
    error: libraryState.error,
    fetchLibrary,
    addToLibrary,
    removeFromLibrary,
    toggleFavorite,
    updateProgress,
    // fetchContinueWatching, // Disabled - endpoint not implemented
    fetchFavorites,
  };
}
