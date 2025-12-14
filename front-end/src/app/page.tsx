'use client';

import { useEffect } from 'react';
import { useMovies } from '@/hooks/useMovies';
import { useLibrary } from '@/hooks/useLibrary';
import MovieRow from '@/components/movie/MovieRow';
import { Skeleton } from '@/components/ui/skeleton';

export default function HomePage() {
  const { trending, popular, topRated, isLoading, fetchTrending, fetchPopular, fetchTopRated } = useMovies();
  const { continueWatching, fetchContinueWatching } = useLibrary();

  useEffect(() => {
    fetchTrending();
    fetchPopular();
    fetchTopRated();
    fetchContinueWatching();
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
      {/* Hero Section */}
      {trending.length > 0 && (
        <div className="relative h-96 rounded-lg overflow-hidden">
          <div
            className="absolute inset-0 bg-cover bg-center"
            style={{ backgroundImage: `url(${trending[0].backdrop || trending[0].poster})` }}
          >
            <div className="absolute inset-0 bg-gradient-to-t from-background via-background/50 to-transparent" />
          </div>
          <div className="relative h-full flex items-end p-8">
            <div className="space-y-4 max-w-2xl">
              <h1 className="text-5xl font-bold">{trending[0].title}</h1>
              <p className="text-lg text-muted-foreground line-clamp-3">
                {trending[0].description}
              </p>
              <div className="flex gap-4">
                <a
                  href={`/movie/${trending[0].id}`}
                  className="px-6 py-3 bg-primary text-primary-foreground rounded-lg font-semibold hover:bg-primary/90 transition-colors"
                >
                  Watch Now
                </a>
                <a
                  href={`/movie/${trending[0].id}`}
                  className="px-6 py-3 bg-secondary text-secondary-foreground rounded-lg font-semibold hover:bg-secondary/80 transition-colors"
                >
                  More Info
                </a>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Continue Watching */}
      {continueWatching.length > 0 && (
        <MovieRow
          title="Continue Watching"
          movies={continueWatching.map((item) => item.movie)}
          showProgress
        />
      )}

      {/* Trending */}
      <MovieRow title="Trending Now" movies={trending} />

      {/* Popular */}
      <MovieRow title="Popular Movies" movies={popular} />

      {/* Top Rated */}
      <MovieRow title="Top Rated" movies={topRated} />
    </div>
  );
}
