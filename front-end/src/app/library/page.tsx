'use client';

import { useEffect } from 'react';
import { useUser } from '@/hooks/useUser';
import { useAuth } from '@/hooks/useAuth';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import Image from 'next/image';
import MovieCard from '@/components/movie/MovieCard';
import { Skeleton } from '@/components/ui/skeleton';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Button } from '@/components/ui/button';
import { Film, Heart, Clock, Bookmark, X, Star, Play } from 'lucide-react';

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
    removeFromLibrary,
    toggleFavorite,
    removeFromWatchLater,
  } = useUser();

  useEffect(() => {
    if (authLoading) return;
    if (!isAuthenticated) {
      router.push('/auth/login');
      return;
    }
    loadUserData();
  }, [isAuthenticated, authLoading, loadUserData, router]);

  const continueWatching = library
    .filter((item) => item.progress > 0 && item.progress < 95)
    .sort((a, b) => new Date(b.lastWatched).getTime() - new Date(a.lastWatched).getTime());

  const allLibraryMovies = [...library].sort(
    (a, b) => new Date(b.addedAt).getTime() - new Date(a.addedAt).getTime()
  );

  const favorites = library
    .filter((item) => item.isFavorite)
    .sort((a, b) => new Date(b.lastWatched).getTime() - new Date(a.lastWatched).getTime());

  const formatDate = (iso: string) =>
    new Date(iso).toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' });

  const formatTime = (seconds: number) => {
    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const s = Math.floor(seconds % 60);
    return h > 0
      ? `${h}:${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`
      : `${m}:${s.toString().padStart(2, '0')}`;
  };

  if (authLoading || (isLoading && library.length === 0)) {
    return (
      <div className="container mx-auto px-4 py-8">
        <Skeleton className="h-10 w-48 mb-8" />
        <div className="flex gap-2 mb-8">
          {Array.from({ length: 5 }).map((_, i) => <Skeleton key={i} className="h-9 w-28 rounded-md" />)}
        </div>
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

      <Tabs defaultValue="all" className="w-full">
        <TabsList className="mb-8 flex-wrap h-auto gap-1">
          <TabsTrigger value="all" className="gap-2">
            <Film className="w-4 h-4" />
            All Movies
            {library.length > 0 && <Badge>{library.length}</Badge>}
          </TabsTrigger>
          <TabsTrigger value="continue" className="gap-2">
            <Clock className="w-4 h-4" />
            Continue Watching
            {continueWatching.length > 0 && <Badge>{continueWatching.length}</Badge>}
          </TabsTrigger>
          <TabsTrigger value="favorites" className="gap-2">
            <Heart className="w-4 h-4" />
            Favorites
            {favorites.length > 0 && <Badge>{favorites.length}</Badge>}
          </TabsTrigger>
          <TabsTrigger value="watchlater" className="gap-2">
            <Bookmark className="w-4 h-4" />
            Watch Later
            {watchLater.length > 0 && <Badge>{watchLater.length}</Badge>}
          </TabsTrigger>
          <TabsTrigger value="history" className="gap-2">
            <Clock className="w-4 h-4" />
            History
            {watchHistory.length > 0 && <Badge>{watchHistory.length}</Badge>}
          </TabsTrigger>
        </TabsList>

        {/* ── All Library ──────────────────────────────────────── */}
        <TabsContent value="all">
          {allLibraryMovies.length > 0 ? (
            <div className="space-y-4">
              <p className="text-muted-foreground text-sm">
                {allLibraryMovies.length} {allLibraryMovies.length === 1 ? 'movie' : 'movies'} in your library
              </p>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
                {allLibraryMovies.map((item) => (
                  <div key={item.id} className="relative group">
                    <MovieCard
                      movie={item.movie}
                      showProgress={item.progress > 0}
                      progress={item.progress}
                    />
                    {/* Actions */}
                    <div className="absolute top-2 right-2 flex flex-col gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                      <Button
                        size="icon"
                        variant="secondary"
                        className="h-7 w-7 rounded-full bg-black/70 hover:bg-red-600 border-0"
                        title={item.isFavorite ? 'Remove from favorites' : 'Add to favorites'}
                        onClick={() => toggleFavorite(item.movieId)}
                      >
                        <Heart className={`h-3.5 w-3.5 ${item.isFavorite ? 'fill-red-400 text-red-400' : ''}`} />
                      </Button>
                      <Button
                        size="icon"
                        variant="secondary"
                        className="h-7 w-7 rounded-full bg-black/70 hover:bg-red-600 border-0"
                        title="Remove from library"
                        onClick={() => removeFromLibrary(item.movieId)}
                      >
                        <X className="h-3.5 w-3.5" />
                      </Button>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <EmptyState icon={<Film className="w-16 h-16" />} title="Your library is empty" description="Start watching movies to build your collection" />
          )}
        </TabsContent>

        {/* ── Continue Watching ─────────────────────────────────── */}
        <TabsContent value="continue">
          {continueWatching.length > 0 ? (
            <div className="space-y-4">
              <p className="text-muted-foreground text-sm">Pick up where you left off</p>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
                {continueWatching.map((item) => (
                  <div key={item.id} className="relative group">
                    <MovieCard movie={item.movie} showProgress progress={item.progress} />
                    <div className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity">
                      <Button
                        size="icon"
                        variant="secondary"
                        className="h-7 w-7 rounded-full bg-black/70 hover:bg-red-600 border-0"
                        title="Remove from library"
                        onClick={() => removeFromLibrary(item.movieId)}
                      >
                        <X className="h-3.5 w-3.5" />
                      </Button>
                    </div>
                    <div className="absolute top-2 left-2 bg-black/70 text-white text-xs px-2 py-0.5 rounded">
                      {Math.round(item.progress)}%
                    </div>
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <EmptyState icon={<Clock className="w-16 h-16" />} title="No movies in progress" description="Start watching a movie to see it here" />
          )}
        </TabsContent>

        {/* ── Favorites ─────────────────────────────────────────── */}
        <TabsContent value="favorites">
          {favorites.length > 0 ? (
            <div className="space-y-4">
              <p className="text-muted-foreground text-sm">{favorites.length} favorite {favorites.length === 1 ? 'movie' : 'movies'}</p>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
                {favorites.map((item) => (
                  <div key={item.id} className="relative group">
                    <MovieCard movie={item.movie} />
                    <div className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity">
                      <Button
                        size="icon"
                        variant="secondary"
                        className="h-7 w-7 rounded-full bg-black/70 hover:bg-red-600 border-0"
                        title="Remove from favorites"
                        onClick={() => toggleFavorite(item.movieId)}
                      >
                        <Heart className="h-3.5 w-3.5 fill-red-400 text-red-400" />
                      </Button>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <EmptyState icon={<Heart className="w-16 h-16" />} title="No favorites yet" description="Heart a movie on its detail page to see it here" />
          )}
        </TabsContent>

        {/* ── Watch Later ───────────────────────────────────────── */}
        <TabsContent value="watchlater">
          {watchLater.length > 0 ? (
            <div className="space-y-4">
              <p className="text-muted-foreground text-sm">{watchLater.length} saved {watchLater.length === 1 ? 'movie' : 'movies'}</p>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
                {watchLater.map((movie) => (
                  <div key={movie.id} className="relative group">
                    <MovieCard movie={movie} />
                    <div className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity">
                      <Button
                        size="icon"
                        variant="secondary"
                        className="h-7 w-7 rounded-full bg-black/70 hover:bg-red-600 border-0"
                        title="Remove from watch later"
                        onClick={() => removeFromWatchLater(movie.id)}
                      >
                        <X className="h-3.5 w-3.5" />
                      </Button>
                    </div>
                    <Bookmark className="absolute top-2 left-2 w-5 h-5 fill-primary text-primary drop-shadow" />
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <EmptyState icon={<Bookmark className="w-16 h-16" />} title="Nothing saved for later" description="Bookmark movies on their detail page" />
          )}
        </TabsContent>

        {/* ── Watch History ─────────────────────────────────────── */}
        <TabsContent value="history">
          {watchHistory.length > 0 ? (
            <div className="space-y-4">
              <p className="text-muted-foreground text-sm">{watchHistory.length} watched {watchHistory.length === 1 ? 'movie' : 'movies'}</p>
              <div className="space-y-3">
                {watchHistory.map((entry) => (
                  <Link
                    key={entry.id}
                    href={`/movie/${entry.movieId}`}
                    className="flex gap-4 items-center p-3 rounded-lg bg-card hover:bg-card/80 transition-colors group"
                  >
                    <div className="relative w-16 aspect-[2/3] shrink-0 rounded overflow-hidden">
                      <Image
                        src={entry.movie?.poster || '/placeholder.jpg'}
                        alt={entry.movie?.title || 'Movie'}
                        fill
                        unoptimized
                        className="object-cover"
                      />
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="font-semibold truncate group-hover:text-primary transition-colors">
                        {entry.movie?.title || 'Unknown Movie'}
                      </p>
                      <p className="text-xs text-muted-foreground mt-1">
                        Watched {formatDate(entry.watchedAt)}
                      </p>
                      {entry.progress > 0 && (
                        <div className="mt-2 space-y-1">
                          <div className="w-full bg-secondary rounded-full h-1">
                            <div
                              className="bg-primary h-1 rounded-full"
                              style={{ width: `${Math.min(100, entry.progress)}%` }}
                            />
                          </div>
                          <p className="text-xs text-muted-foreground">{Math.round(entry.progress)}% watched</p>
                        </div>
                      )}
                    </div>
                    <div className="flex items-center gap-2 shrink-0">
                      {entry.movie?.rating != null && (
                        <span className="flex items-center gap-1 text-xs text-muted-foreground">
                          <Star className="h-3 w-3 fill-yellow-400 text-yellow-400" />
                          {entry.movie.rating.toFixed(1)}
                        </span>
                      )}
                      <Button size="sm" variant="ghost" className="gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                        <Play className="h-3.5 w-3.5" />
                        Play
                      </Button>
                    </div>
                  </Link>
                ))}
              </div>
            </div>
          ) : (
            <EmptyState icon={<Clock className="w-16 h-16" />} title="No watch history" description="Movies you watch will appear here" />
          )}
        </TabsContent>
      </Tabs>
    </div>
  );
}

function Badge({ children }: { children: React.ReactNode }) {
  return (
    <span className="ml-1 px-2 py-0.5 text-xs bg-primary text-primary-foreground rounded-full">
      {children}
    </span>
  );
}

function EmptyState({
  icon,
  title,
  description,
}: {
  icon: React.ReactNode;
  title: string;
  description: string;
}) {
  return (
    <div className="text-center py-20">
      <div className="flex justify-center mb-4 text-muted-foreground opacity-40">{icon}</div>
      <h3 className="text-2xl font-semibold mb-2">{title}</h3>
      <p className="text-muted-foreground">{description}</p>
    </div>
  );
}
