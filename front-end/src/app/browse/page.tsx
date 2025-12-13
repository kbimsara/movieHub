'use client';

import { useState, useEffect } from 'react';
import { useMovies } from '@/hooks/useMovies';
import SearchBar from '@/components/movie/SearchBar';
import MovieGrid from '@/components/movie/MovieGrid';
import { SearchFilters } from '@/types';

export default function BrowsePage() {
  const { movies, genres, isLoading, fetchMovies, fetchGenres, searchMovies } = useMovies();
  const [filters, setFilters] = useState<SearchFilters>({});

  useEffect(() => {
    fetchGenres();
    fetchMovies();
  }, []);

  const handleSearch = (newFilters: SearchFilters) => {
    setFilters(newFilters);
    if (newFilters.query) {
      searchMovies(newFilters.query);
    } else {
      fetchMovies(newFilters, { page: 1, limit: 24 });
    }
  };

  return (
    <div className="container mx-auto px-4 py-8 space-y-8">
      <div className="space-y-4">
        <h1 className="text-4xl font-bold">Browse Movies</h1>
        <SearchBar onSearch={handleSearch} genres={genres} />
      </div>

      <MovieGrid movies={movies} isLoading={isLoading} />
    </div>
  );
}
