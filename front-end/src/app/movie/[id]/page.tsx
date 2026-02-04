'use client';

import { useEffect, useState } from 'react';
import { useParams } from 'next/navigation';
import { useAppDispatch, useAppSelector } from '@/hooks/redux';
import { useUser } from '@/hooks/useUser';
import { setCurrentMovie } from '@/store/slices/movieSlice';
import { movieService } from '@/services/movie.service';
import VideoPlayer from '@/components/player/VideoPlayer';
import MovieCard from '@/components/movie/MovieCard';
import TorrentSeedWidget from '@/components/torrent/TorrentSeedWidget';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import {
  Play,
  Plus,
  Check,
  Heart,
  Download,
  Share2,
  Clock,
  Star,
  Calendar,
} from 'lucide-react';
import { Movie } from '@/types';

export default function MovieDetailPage() {
  const params = useParams();
  const dispatch = useAppDispatch();
  const movieId = params.id as string;

  const currentMovie = useAppSelector((state) => state.movie.currentMovie);
  const { library, addToLibrary, removeFromLibrary, toggleFavorite, updateProgress } = useUser();

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [relatedMovies, setRelatedMovies] = useState<Movie[]>([]);
  const [isPlaying, setIsPlaying] = useState(false);

  const libraryItem = library.find((item) => item.movieId === movieId);
  const isInLibrary = !!libraryItem;
  const isFavorite = libraryItem?.isFavorite || false;

  useEffect(() => {
    loadMovie();
  }, [movieId]);

  const loadMovie = async () => {
    try {
      setIsLoading(true);
      setError(null);

      const [movieRes, relatedRes] = await Promise.allSettled([
        movieService.getMovieById(movieId),
        movieService.getRelatedMovies(movieId),
      ]);

      if (movieRes.status === 'fulfilled' && movieRes.value.success && movieRes.value.data) {
        dispatch(setCurrentMovie(movieRes.value.data));
      } else {
        dispatch(setCurrentMovie(null));
        setError('Movie not found');
      }

      if (relatedRes.status === 'fulfilled' && relatedRes.value.success) {
        setRelatedMovies(relatedRes.value.data || []);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to load movie');
    } finally {
      setIsLoading(false);
    }
  };

  const handlePlayClick = () => {
    setIsPlaying(true);
    if (!isInLibrary) {
      addToLibrary(movieId);
    }
  };

  const handleProgressUpdate = (progress: number) => {
    if (isInLibrary) {
      updateProgress(movieId, progress);
    }
  };

  const handleToggleLibrary = () => {
    if (isInLibrary) {
      removeFromLibrary(movieId);
    } else {
      addToLibrary(movieId);
    }
  };

  const handleToggleFavorite = () => {
    if (!isInLibrary) {
      addToLibrary(movieId);
    }
    toggleFavorite(movieId);
  };

  if (isLoading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <Skeleton className="w-full aspect-video rounded-lg mb-8" />
        <Skeleton className="h-12 w-2/3 mb-4" />
        <Skeleton className="h-6 w-full mb-2" />
        <Skeleton className="h-6 w-5/6" />
      </div>
    );
  }

  if (error || !currentMovie) {
    return (
      <div className="container mx-auto px-4 py-20 text-center">
        <h2 className="text-3xl font-bold mb-4">Movie Not Found</h2>
        <p className="text-muted-foreground">{error || 'The movie you are looking for does not exist.'}</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen">
      {/* Video Player or Backdrop */}
      {isPlaying && currentMovie.streamUrl ? (
        <div className="w-full bg-black">
          <div className="container mx-auto">
            <VideoPlayer movie={currentMovie} onProgressUpdate={handleProgressUpdate} />
          </div>
        </div>
      ) : (
        <div
          className="w-full aspect-video bg-cover bg-center relative"
          style={{
            backgroundImage: `linear-gradient(to top, rgb(0, 0, 0), transparent), url(${currentMovie.backdrop || currentMovie.poster})`,
          }}
        >
          <div className="absolute inset-0 bg-gradient-to-t from-background via-background/50 to-transparent" />
          <div className="absolute bottom-0 left-0 right-0 p-8 container mx-auto">
            <h1 className="text-5xl font-bold mb-4">{currentMovie.title}</h1>
            <div className="flex items-center gap-4 mb-6">
              <span className="flex items-center gap-1">
                <Star className="w-5 h-5 fill-yellow-500 text-yellow-500" />
                {currentMovie.rating.toFixed(1)}
              </span>
              <span className="flex items-center gap-1">
                <Calendar className="w-5 h-5" />
                {currentMovie.year}
              </span>
              <span className="flex items-center gap-1">
                <Clock className="w-5 h-5" />
                {currentMovie.duration} min
              </span>
              <span className="px-3 py-1 bg-primary/20 rounded text-sm">
                {currentMovie.quality}
              </span>
            </div>

            <div className="flex items-center gap-3">
              <Button size="lg" className="gap-2" onClick={handlePlayClick}>
                <Play className="w-5 h-5" />
                {libraryItem?.progress && libraryItem.progress > 5
                  ? `Continue (${Math.round(libraryItem.progress)}%)`
                  : 'Play Now'}
              </Button>

              <Button
                size="lg"
                variant="outline"
                className="gap-2"
                onClick={handleToggleLibrary}
              >
                {isInLibrary ? (
                  <>
                    <Check className="w-5 h-5" />
                    In Library
                  </>
                ) : (
                  <>
                    <Plus className="w-5 h-5" />
                    Add to Library
                  </>
                )}
              </Button>

              <Button
                size="lg"
                variant="outline"
                className="gap-2"
                onClick={handleToggleFavorite}
              >
                <Heart
                  className={`w-5 h-5 ${
                    isFavorite ? 'fill-red-500 text-red-500' : ''
                  }`}
                />
              </Button>

              {currentMovie.downloadUrl && (
                <Button size="lg" variant="outline" className="gap-2">
                  <Download className="w-5 h-5" />
                </Button>
              )}

              <Button size="lg" variant="outline" className="gap-2">
                <Share2 className="w-5 h-5" />
              </Button>
            </div>
          </div>
        </div>
      )}

      {/* Movie Details */}
      <div className="container mx-auto px-4 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Main Content */}
          <div className="lg:col-span-2 space-y-6">
            <Tabs defaultValue="overview" className="w-full">
              <TabsList>
                <TabsTrigger value="overview">Overview</TabsTrigger>
                <TabsTrigger value="cast">Cast & Crew</TabsTrigger>
                {currentMovie.torrentMagnet && (
                  <TabsTrigger value="torrent">Torrent</TabsTrigger>
                )}
              </TabsList>

              <TabsContent value="overview" className="space-y-4">
                <div>
                  <h3 className="text-2xl font-semibold mb-3">Synopsis</h3>
                  <p className="text-muted-foreground leading-relaxed">
                    {currentMovie.description}
                  </p>
                </div>

                {currentMovie.genres && currentMovie.genres.length > 0 && (
                  <div>
                    <h3 className="text-xl font-semibold mb-3">Genres</h3>
                    <div className="flex flex-wrap gap-2">
                      {currentMovie.genres.map((genre) => (
                        <span
                          key={genre}
                          className="px-3 py-1 bg-secondary rounded-full text-sm"
                        >
                          {genre}
                        </span>
                      ))}
                    </div>
                  </div>
                )}

                {currentMovie.tags && currentMovie.tags.length > 0 && (
                  <div>
                    <h3 className="text-xl font-semibold mb-3">Tags</h3>
                    <div className="flex flex-wrap gap-2">
                      {currentMovie.tags.map((tag) => (
                        <span
                          key={tag}
                          className="px-3 py-1 bg-secondary/50 rounded text-sm"
                        >
                          {tag}
                        </span>
                      ))}
                    </div>
                  </div>
                )}
              </TabsContent>

              <TabsContent value="cast" className="space-y-4">
                {currentMovie.director && (
                  <div>
                    <h3 className="text-xl font-semibold mb-2">Director</h3>
                    <p className="text-muted-foreground">{currentMovie.director}</p>
                  </div>
                )}

                {currentMovie.cast && currentMovie.cast.length > 0 && (
                  <div>
                    <h3 className="text-xl font-semibold mb-4">Cast</h3>
                    <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                      {currentMovie.cast.map((member) => (
                        <div key={member.id} className="flex items-center gap-3">
                          <div className="w-12 h-12 rounded-full bg-secondary" />
                          <div>
                            <p className="font-medium">{member.name}</p>
                            <p className="text-sm text-muted-foreground">
                              {member.character}
                            </p>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </TabsContent>

              {currentMovie.torrentMagnet && (
                <TabsContent value="torrent">
                  <TorrentSeedWidget
                    movieId={currentMovie.id}
                    movieTitle={currentMovie.title}
                  />
                </TabsContent>
              )}
            </Tabs>
          </div>

          {/* Sidebar */}
          <div className="space-y-6">
            <div className="bg-card border rounded-lg p-6 space-y-4">
              <h3 className="font-semibold">Movie Info</h3>
              <div className="space-y-2 text-sm">
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Rating</span>
                  <span className="font-medium">{currentMovie.rating}/10</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Year</span>
                  <span className="font-medium">{currentMovie.year}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Duration</span>
                  <span className="font-medium">{currentMovie.duration} min</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Quality</span>
                  <span className="font-medium">{currentMovie.quality}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Views</span>
                  <span className="font-medium">{currentMovie.views.toLocaleString()}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Related Movies */}
        {relatedMovies.length > 0 && (
          <div className="mt-12">
            <h3 className="text-2xl font-bold mb-6">More Like This</h3>
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-6">
              {relatedMovies.map((movie) => (
                <MovieCard key={movie.id} movie={movie} />
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
