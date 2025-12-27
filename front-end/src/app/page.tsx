'use client';

import { useEffect } from 'react';
import { useMovies } from '@/hooks/useMovies';
import { useLibrary } from '@/hooks/useLibrary';
import MovieRow from '@/components/movie/MovieRow';
import { Skeleton } from '@/components/ui/skeleton';

export default function HomePage() {
  // NOTE: fetchTrending, fetchPopular, fetchTopRated are disabled (endpoints not implemented)
  // Using fetchMovies() as fallback to get all movies
  const { movies, isLoading, fetchMovies } = useMovies();
  const { library } = useLibrary();

  useEffect(() => {
    // Fetch all movies since trending/popular/topRated endpoints don't exist yet
    fetchMovies();
  }, []);

  if (isLoading) {
    return (
      <div className="container mx-auto px-4 py-8 space-y-8">
        <Skeleton className="h-96 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8 space-y-12">
      {/* Hero Section - Using first movie from catalog */}
      {movies.length > 0 && (
        <div className="relative h-96 rounded-lg overflow-hidden">
          <div
            className="absolute inset-0 bg-cover bg-center"
            style={{ backgroundImage: `url(${movies[0].backdrop || movies[0].poster})` }}
          >
            <div className="absolute inset-0 bg-gradient-to-t from-background via-background/50 to-transparent" />
          </div>
          <div className="relative h-full flex items-end p-8">
            <div className="space-y-4 max-w-2xl">
              <h1 className="text-5xl font-bold">{movies[0].title}</h1>
              <p className="text-lg text-muted-foreground line-clamp-3">
                {movies[0].description}
              </p>
              <div className="flex gap-4">
                <a
                  href={`/movie/${movies[0].id}`}
                  className="px-6 py-3 bg-primary text-primary-foreground rounded-lg font-semibold hover:bg-primary/90 transition-colors"
                >
                  Watch Now
                </a>
                <a
                  href={`/movie/${movies[0].id}`}
                  className="px-6 py-3 bg-secondary text-secondary-foreground rounded-lg font-semibold hover:bg-secondary/80 transition-colors"
                >
                  More Info
                </a>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Continue Watching - Disabled (endpoint not implemented) */}
      {/* {continueWatching.length > 0 && (
        <MovieRow
          title="Continue Watching"
          movies={continueWatching.map((item) => item.movie)}
          showProgress
        />
      )} */}

      {/* Library */}
      {library.length > 0 && (
        <MovieRow
          title="My Library"
          movies={library.map((item) => item.movie)}
        />
      )}

      {/* All Movies - Replacing trending/popular/topRated sections */}
      <MovieRow title="Available Movies" movies={movies} />
    </div>
  );
}
