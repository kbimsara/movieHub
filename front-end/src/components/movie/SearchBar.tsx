'use client';

import { useState } from 'react';
import { Search as SearchIcon, SlidersHorizontal } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { SearchFilters } from '@/types';

interface SearchBarProps {
  onSearch: (filters: SearchFilters) => void;
  genres?: string[];
}

export default function SearchBar({ onSearch, genres = [] }: SearchBarProps) {
  const [query, setQuery] = useState('');
  const [showFilters, setShowFilters] = useState(false);
  const [filters, setFilters] = useState<SearchFilters>({});

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    onSearch({ ...filters, query });
  };

  return (
    <div className="w-full">
      <form onSubmit={handleSearch} className="flex gap-2">
        <div className="relative flex-1">
          <SearchIcon className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            type="text"
            placeholder="Search movies..."
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            className="pl-10"
          />
        </div>
        <Button type="submit">Search</Button>
        <Button
          type="button"
          variant="outline"
          size="icon"
          onClick={() => setShowFilters(!showFilters)}
        >
          <SlidersHorizontal className="h-4 w-4" />
        </Button>
      </form>

      {showFilters && (
        <div className="mt-4 p-4 border rounded-lg bg-card">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {/* Genre Filter */}
            {genres.length > 0 && (
              <div>
                <label className="text-sm font-medium mb-2 block">Genre</label>
                <select
                  className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                  onChange={(e) => setFilters({ ...filters, genres: [e.target.value] })}
                >
                  <option value="">All Genres</option>
                  {genres.map((genre) => (
                    <option key={genre} value={genre}>
                      {genre}
                    </option>
                  ))}
                </select>
              </div>
            )}

            {/* Year Filter */}
            <div>
              <label className="text-sm font-medium mb-2 block">Year</label>
              <Input
                type="number"
                placeholder="e.g., 2023"
                onChange={(e) => setFilters({ ...filters, year: parseInt(e.target.value) || undefined })}
              />
            </div>

            {/* Sort By */}
            <div>
              <label className="text-sm font-medium mb-2 block">Sort By</label>
              <select
                className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                onChange={(e) =>
                  setFilters({ ...filters, sortBy: e.target.value as any })
                }
              >
                <option value="createdAt">Recently Added</option>
                <option value="rating">Rating</option>
                <option value="year">Year</option>
                <option value="title">Title</option>
                <option value="views">Most Viewed</option>
              </select>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
