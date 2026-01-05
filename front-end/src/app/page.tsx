'use client';

import { useEffect } from 'react';
import { useMovies } from '@/hooks/useMovies';
import { useLibrary } from '@/hooks/useLibrary';
import MovieRow from '@/components/movie/MovieRow';
import { Skeleton } from '@/components/ui/skeleton';
import { Button } from '@/components/ui/button';
import { Upload, Play, Info } from 'lucide-react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';

export default function HomePage() {
  const router = useRouter();
  const { isAuthenticated } = useAuth();
  // NOTE: fetchTrending, fetchPopular, fetchTopRated are disabled (endpoints not implemented)
  // Using fetchMovies() as fallback to get all movies
  const { movies, isLoading, fetchMovies } = useMovies();
  const { library } = useLibrary();

  useEffect(() => {
    // Fetch all movies since trending/popular/topRated endpoints don't exist yet
    fetchMovies();
  }, []);

  // Sort movies by creation date (newest first) to show recently uploaded
  const recentlyAdded = [...movies].sort((a, b) => 
    new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
  ).slice(0, 12);

  const featuredMovie = movies[0];

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
      {featuredMovie && (
        <div className="relative h-96 rounded-lg overflow-hidden">
          <div
            className="absolute inset-0 bg-cover bg-center"
            style={{ backgroundImage: `url(${featuredMovie.backdrop || featuredMovie.poster})` }}
          >
            <div className="absolute inset-0 bg-gradient-to-t from-background via-background/50 to-transparent" />
          </div>
          <div className="relative h-full flex items-end p-8">
            <div className="space-y-4 max-w-2xl">
              <h1 className="text-5xl font-bold">{featuredMovie.title}</h1>
              <p className="text-lg text-muted-foreground line-clamp-3">
                {featuredMovie.description}
              </p>
              <div className="flex items-center gap-4">
                <Button
                  size="lg"
                  onClick={() => router.push(`/movie/${featuredMovie.id}`)}
                  className="gap-2"
                >
                  <Play className="h-5 w-5" />
                  Watch Now
                </Button>
                <Button
                  size="lg"
                  variant="secondary"
                  onClick={() => router.push(`/movie/${featuredMovie.id}`)}
                  className="gap-2"
                >
                  <Info className="h-5 w-5" />
                  More Info
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Upload CTA for authenticated users */}
      {isAuthenticated && movies.length === 0 && !isLoading && (
        <div className="text-center py-16 space-y-6">
          <div className="mx-auto w-20 h-20 rounded-full bg-primary/10 flex items-center justify-center">
            <Upload className="h-10 w-10 text-primary" />
          </div>
          <div className="space-y-2">
            <h2 className="text-3xl font-bold">No Movies Yet</h2>
            <p className="text-muted-foreground max-w-md mx-auto">
              Be the first to share a movie! Upload your favorite films and share them with the community.
            </p>
          </div>
          <Button size="lg" onClick={() => router.push('/upload')} className="gap-2">
            <Upload className="h-5 w-5" />
            Upload Your First Movie
          </Button>
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

      {/* Recently Added - Shows newest uploads first */}
      {recentlyAdded.length > 0 && (
        <MovieRow 
          title="Recently Added" 
          movies={recentlyAdded}
          description="Latest movies uploaded by the community"
        />
      )}

      {/* Library */}
      {library.length > 0 && (
        <MovieRow
          title="My Library"
          movies={library.map((item) => item.movie)}
        />
      )}

      {/* All Movies - Replacing trending/popular/topRated sections */}
      {movies.length > 0 && (
        <MovieRow 
          title="All Movies" 
          movies={movies}
          description="Browse the complete catalog"
        />
      )}
    </div>
  );
}
