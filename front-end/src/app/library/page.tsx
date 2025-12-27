'use client';

import { useEffect } from 'react';
import { useUser } from '@/hooks/useUser';
import { useAuth } from '@/hooks/useAuth';
import { useRouter } from 'next/navigation';
import MovieCard from '@/components/movie/MovieCard';
import { Skeleton } from '@/components/ui/skeleton';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Film, Heart, Clock, Bookmark } from 'lucide-react';

export default function LibraryPage() {
  const router = useRouter();
  const { isAuthenticated, isLoading: authLoading } = useAuth();
  const {
    library,
    watchHistory,
    favoriteMovies,
    watchLater,
    isLoading,
    loadUserData,
  } = useUser();

  useEffect(() => {
    if (!authLoading && !isAuthenticated) {
      router.push('/auth/login');
    }
  }, [isAuthenticated, authLoading, router]);

  useEffect(() => {
    if (isAuthenticated) {
      loadUserData();
    }
  }, [isAuthenticated]);

  // Filter movies by progress
  const continueWatching = library
    .filter((item) => item.progress > 0 && item.progress < 95)
    .sort((a, b) => new Date(b.lastWatched).getTime() - new Date(a.lastWatched).getTime());

  const allLibraryMovies = library
    .sort((a, b) => new Date(b.addedAt).getTime() - new Date(a.addedAt).getTime())
    .map((item) => item.movie);

  const favorites = library
    .filter((item) => item.isFavorite)
    .sort((a, b) => new Date(b.lastWatched).getTime() - new Date(a.lastWatched).getTime())
    .map((item) => item.movie);

  if (authLoading || (isLoading && library.length === 0)) {
    return (
      <div className="container mx-auto px-4 py-8">
        <Skeleton className="h-10 w-48 mb-8" />
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
          {Array.from({ length: 10 }).map((_, i) => (
            <div key={i} className="space-y-3">
              <Skeleton className="aspect-[2/3] rounded-lg" />
              <Skeleton className="h-4 w-3/4" />
              <Skeleton className="h-3 w-1/2" />
            </div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold mb-8">My Library</h1>

      <Tabs defaultValue="continue" className="w-full">
        <TabsList className="mb-8">
          <TabsTrigger value="continue" className="gap-2">
            <Clock className="w-4 h-4" />
            Continue Watching
            {continueWatching.length > 0 && (
              <span className="ml-1 px-2 py-0.5 text-xs bg-primary text-primary-foreground rounded-full">
                {continueWatching.length}
              </span>
            )}
          </TabsTrigger>
          <TabsTrigger value="all" className="gap-2">
            <Film className="w-4 h-4" />
            All Movies
            {library.length > 0 && (
              <span className="ml-1 px-2 py-0.5 text-xs bg-primary text-primary-foreground rounded-full">
                {library.length}
              </span>
            )}
          </TabsTrigger>
          <TabsTrigger value="favorites" className="gap-2">
            <Heart className="w-4 h-4" />
            Favorites
            {favorites.length > 0 && (
              <span className="ml-1 px-2 py-0.5 text-xs bg-primary text-primary-foreground rounded-full">
                {favorites.length}
              </span>
            )}
          </TabsTrigger>
          <TabsTrigger value="watchlater" className="gap-2">
            <Bookmark className="w-4 h-4" />
            Watch Later
            {watchLater.length > 0 && (
              <span className="ml-1 px-2 py-0.5 text-xs bg-primary text-primary-foreground rounded-full">
                {watchLater.length}
              </span>
            )}
          </TabsTrigger>
        </TabsList>

        {/* Continue Watching */}
        <TabsContent value="continue">
          {continueWatching.length > 0 ? (
            <div className="space-y-6">
              <p className="text-muted-foreground">
                Pick up where you left off
              </p>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
                {continueWatching.map((item) => (
                  <div key={item.id} className="relative">
                    <MovieCard movie={item.movie} />
                    <div className="absolute bottom-0 left-0 right-0 h-1 bg-secondary/50 rounded-b-lg overflow-hidden">
                      <div
                        className="h-full bg-primary transition-all"
                        style={{ width: `${item.progress}%` }}
                      />
                    </div>
                    <div className="absolute top-2 right-2 px-2 py-1 bg-black/80 rounded text-xs">
                      {Math.round(item.progress)}%
                    </div>
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <div className="text-center py-20">
              <Clock className="w-16 h-16 mx-auto mb-4 text-muted-foreground opacity-50" />
              <h3 className="text-2xl font-semibold mb-2">No movies in progress</h3>
              <p className="text-muted-foreground">
                Start watching a movie to see it here
              </p>
            </div>
          )}
        </TabsContent>

        {/* All Library Movies */}
        <TabsContent value="all">
          {allLibraryMovies.length > 0 ? (
            <div className="space-y-6">
              <p className="text-muted-foreground">
                {allLibraryMovies.length} {allLibraryMovies.length === 1 ? 'movie' : 'movies'} in your library
              </p>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
                {allLibraryMovies.map((movie) => (
                  <MovieCard key={movie.id} movie={movie} />
                ))}
              </div>
            </div>
          ) : (
            <div className="text-center py-20">
              <Film className="w-16 h-16 mx-auto mb-4 text-muted-foreground opacity-50" />
              <h3 className="text-2xl font-semibold mb-2">Your library is empty</h3>
              <p className="text-muted-foreground">
                Add movies to build your personal collection
              </p>
            </div>
          )}
        </TabsContent>

        {/* Favorites */}
        <TabsContent value="favorites">
          {favorites.length > 0 ? (
            <div className="space-y-6">
              <p className="text-muted-foreground">
                Your favorite movies
              </p>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
                {favorites.map((movie) => (
                  <div key={movie.id} className="relative">
                    <MovieCard movie={movie} />
                    <Heart className="absolute top-2 right-2 w-6 h-6 fill-red-500 text-red-500" />
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <div className="text-center py-20">
              <Heart className="w-16 h-16 mx-auto mb-4 text-muted-foreground opacity-50" />
              <h3 className="text-2xl font-semibold mb-2">No favorites yet</h3>
              <p className="text-muted-foreground">
                Mark movies as favorites to see them here
              </p>
            </div>
          )}
        </TabsContent>

        {/* Watch Later */}
        <TabsContent value="watchlater">
          {watchLater.length > 0 ? (
            <div className="space-y-6">
              <p className="text-muted-foreground">
                Movies you want to watch later
              </p>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
                {watchLater.map((movie) => (
                  <div key={movie.id} className="relative">
                    <MovieCard movie={movie} />
                    <Bookmark className="absolute top-2 right-2 w-6 h-6 fill-primary text-primary" />
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <div className="text-center py-20">
              <Bookmark className="w-16 h-16 mx-auto mb-4 text-muted-foreground opacity-50" />
              <h3 className="text-2xl font-semibold mb-2">No saved movies</h3>
              <p className="text-muted-foreground">
                Bookmark movies to watch them later
              </p>
            </div>
          )}
        </TabsContent>
      </Tabs>
    </div>
  );
}
    </div>
  );
}
