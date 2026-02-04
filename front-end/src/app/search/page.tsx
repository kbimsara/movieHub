'use client';

import { useState, useEffect } from 'react';
import { useSearch } from '@/hooks/useSearch';
import { useAppSelector } from '@/hooks/redux';
import MovieGrid from '@/components/movie/MovieGrid';
import SearchBar from '@/components/movie/SearchBar';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Skeleton } from '@/components/ui/skeleton';
import { Search, Filter, X } from 'lucide-react';
import { SearchFilters } from '@/types';

const GENRES = [
  'Action',
  'Comedy',
  'Drama',
  'Horror',
  'Sci-Fi',
  'Thriller',
  'Romance',
  'Documentary',
  'Animation',
  'Fantasy',
  'Crime',
  'Mystery',
];

const YEARS = Array.from({ length: 30 }, (_, i) => new Date().getFullYear() - i);

const QUALITIES = ['480p', '720p', '1080p', '4K'];

export default function SearchPage() {
  const {
    results,
    isLoading,
    error,
    total,
    hasMore,
    searchMovies,
    updateFilters,
    loadMore,
  } = useSearch();

  const filters = useAppSelector((state) => state.search.filters);

  const [showFilters, setShowFilters] = useState(false);
  const [localFilters, setLocalFilters] = useState(filters);

  useEffect(() => {
    setLocalFilters(filters);
  }, [filters]);

  const handleApplyFilters = () => {
    updateFilters(localFilters);
    setShowFilters(false);
  };

  const handleClearFilters = () => {
    const clearedFilters = {
      genres: [],
      year: undefined,
      quality: [],
      rating: undefined,
      sortBy: 'relevance' as const,
      sortOrder: 'desc' as const,
    };
    setLocalFilters(clearedFilters);
    updateFilters(clearedFilters);
  };

  const toggleGenre = (genre: string) => {
    const newGenres = localFilters.genres?.includes(genre)
      ? localFilters.genres.filter((g) => g !== genre)
      : [...(localFilters.genres || []), genre];
    setLocalFilters({ ...localFilters, genres: newGenres });
  };

  const toggleQuality = (quality: string) => {
    const newQualities = localFilters.quality?.includes(quality)
      ? localFilters.quality.filter((q) => q !== quality)
      : [...(localFilters.quality || []), quality];
    setLocalFilters({ ...localFilters, quality: newQualities });
  };

  const handleSearchSubmit = (searchInput: SearchFilters) => {
    const nextQuery = searchInput.query ?? '';
    searchMovies(nextQuery, true);
    updateFilters(searchInput);
  };

  return (
    <div className="container mx-auto px-4 py-8">
      {/* Search Bar */}
      <div className="mb-8">
        <SearchBar onSearch={handleSearchSubmit} genres={GENRES} />
        
        <div className="flex items-center gap-4 mt-4">
          <Button
            variant="outline"
            onClick={() => setShowFilters(!showFilters)}
            className="gap-2"
          >
            <Filter className="w-4 h-4" />
            Filters
            {(filters.genres?.length || 0) + (filters.quality?.length || 0) > 0 && (
              <span className="ml-1 px-2 py-0.5 text-xs bg-primary text-primary-foreground rounded-full">
                {(filters.genres?.length || 0) + (filters.quality?.length || 0)}
              </span>
            )}
          </Button>

          {((filters.genres?.length || 0) > 0 || (filters.quality?.length || 0) > 0 || filters.year) && (
            <Button variant="ghost" onClick={handleClearFilters} className="gap-2">
              <X className="w-4 h-4" />
              Clear Filters
            </Button>
          )}

          {total > 0 && (
            <p className="text-sm text-muted-foreground ml-auto">
              Found {total.toLocaleString()} movies
            </p>
          )}
        </div>
      </div>

      {/* Filters Panel */}
      {showFilters && (
        <div className="mb-8 p-6 bg-card border rounded-lg space-y-6">
          {/* Genres */}
          <div>
            <Label className="text-base font-semibold mb-3 block">Genres</Label>
            <div className="flex flex-wrap gap-2">
              {GENRES.map((genre) => (
                <button
                  key={genre}
                  onClick={() => toggleGenre(genre)}
                  className={`px-4 py-2 rounded-full text-sm transition-colors ${
                    localFilters.genres?.includes(genre)
                      ? 'bg-primary text-primary-foreground'
                      : 'bg-secondary text-secondary-foreground hover:bg-secondary/80'
                  }`}
                >
                  {genre}
                </button>
              ))}
            </div>
          </div>

          {/* Year */}
          <div>
            <Label className="text-base font-semibold mb-3 block">Year</Label>
            <select
              value={localFilters.year || ''}
              onChange={(e) =>
                setLocalFilters({
                  ...localFilters,
                  year: e.target.value ? parseInt(e.target.value) : undefined,
                })
              }
              className="w-full max-w-xs px-4 py-2 bg-background border rounded-md"
            >
              <option value="">All Years</option>
              {YEARS.map((year) => (
                <option key={year} value={year}>
                  {year}
                </option>
              ))}
            </select>
          </div>

          {/* Quality */}
          <div>
            <Label className="text-base font-semibold mb-3 block">Quality</Label>
            <div className="flex flex-wrap gap-2">
              {QUALITIES.map((quality) => (
                <button
                  key={quality}
                  onClick={() => toggleQuality(quality)}
                  className={`px-4 py-2 rounded-full text-sm transition-colors ${
                    localFilters.quality?.includes(quality)
                      ? 'bg-primary text-primary-foreground'
                      : 'bg-secondary text-secondary-foreground hover:bg-secondary/80'
                  }`}
                >
                  {quality}
                </button>
              ))}
            </div>
          </div>

          {/* Sort */}
          <div>
            <Label className="text-base font-semibold mb-3 block">Sort By</Label>
            <div className="flex gap-4">
              <select
                value={localFilters.sortBy || 'relevance'}
                onChange={(e) =>
                  setLocalFilters({
                    ...localFilters,
                    sortBy: e.target.value as any,
                  })
                }
                className="px-4 py-2 bg-background border rounded-md"
              >
                <option value="relevance">Relevance</option>
                <option value="title">Title</option>
                <option value="year">Year</option>
                <option value="rating">Rating</option>
                <option value="views">Views</option>
                <option value="createdAt">Recently Added</option>
              </select>

              <select
                value={localFilters.sortOrder || 'desc'}
                onChange={(e) =>
                  setLocalFilters({
                    ...localFilters,
                    sortOrder: e.target.value as 'asc' | 'desc',
                  })
                }
                className="px-4 py-2 bg-background border rounded-md"
              >
                <option value="desc">Descending</option>
                <option value="asc">Ascending</option>
              </select>
            </div>
          </div>

          {/* Actions */}
          <div className="flex gap-3 pt-4 border-t">
            <Button onClick={handleApplyFilters} className="flex-1">
              Apply Filters
            </Button>
            <Button variant="outline" onClick={() => setShowFilters(false)} className="flex-1">
              Cancel
            </Button>
          </div>
        </div>
      )}

      {/* Results */}
      {error && (
        <div className="text-center py-12">
          <p className="text-destructive">{error}</p>
        </div>
      )}

      {isLoading && results.length === 0 ? (
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
          {Array.from({ length: 10 }).map((_, i) => (
            <div key={i} className="space-y-3">
              <Skeleton className="aspect-[2/3] rounded-lg" />
              <Skeleton className="h-4 w-3/4" />
              <Skeleton className="h-3 w-1/2" />
            </div>
          ))}
        </div>
      ) : results.length > 0 ? (
        <>
          <MovieGrid movies={results} />
          
          {hasMore && (
            <div className="text-center mt-12">
              <Button
                onClick={loadMore}
                disabled={isLoading}
                size="lg"
                variant="outline"
              >
                {isLoading ? 'Loading...' : 'Load More'}
              </Button>
            </div>
          )}
        </>
      ) : (
        <div className="text-center py-20">
          <Search className="w-16 h-16 mx-auto mb-4 text-muted-foreground opacity-50" />
          <h3 className="text-2xl font-semibold mb-2">No Results Found</h3>
          <p className="text-muted-foreground">
            Try adjusting your search or filters
          </p>
        </div>
      )}
    </div>
  );
}
