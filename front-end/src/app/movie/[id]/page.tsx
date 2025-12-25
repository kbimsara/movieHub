'use client';

import { useEffect } from 'react';
import { useParams } from 'next/navigation';
import Image from 'next/image';
import Link from 'next/link';
import { useMovies } from '@/hooks/useMovies';
import { useLibrary } from '@/hooks/useLibrary';
import { Button } from '@/components/ui/button';
import { Play, Plus, Heart, Download, Star, Clock } from 'lucide-react';
import MovieRow from '@/components/movie/MovieRow';
import TorrentSeedWidget from '@/components/torrent/TorrentSeedWidget';
import { useToast } from '@/components/ui/toaster';
import { Skeleton } from '@/components/ui/skeleton';

export default function MovieDetailPage() {
  const params = useParams();
  const movieId = params.id as string;
  const { currentMovie, relatedMovies, isLoading, fetchMovieById, fetchRelatedMovies } = useMovies();
  const { addToLibrary, toggleFavorite } = useLibrary();
  const { toast } = useToast();

  useEffect(() => {
    if (movieId) {
      fetchMovieById(movieId);
      fetchRelatedMovies(movieId);
    }
  }, [movieId]);

  const handleAddToLibrary = async () => {
    if (!currentMovie) return;
    const result = await addToLibrary(currentMovie.id);
    if (result.success) {
      toast({
        title: 'Success',
        description: 'Movie added to library',
      });
    }
  };

  const handleToggleFavorite = async () => {
    if (!currentMovie) return;
    const result = await toggleFavorite(currentMovie.id);
    if (result.success) {
      toast({
        title: 'Success',
        description: 'Favorite status updated',
      });
    }
  };

  if (isLoading || !currentMovie) {
    return (
      <div className="container mx-auto px-4 py-8 space-y-8">
        <Skeleton className="h-96 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    );
  }

  return (
    <div className="min-h-screen">
      {/* Hero Section with Backdrop */}
      <div className="relative h-screen">
        <div
          className="absolute inset-0 bg-cover bg-center"
          style={{ backgroundImage: `url(${currentMovie.backdrop || currentMovie.poster})` }}
        >
          <div className="absolute inset-0 bg-gradient-to-t from-background via-background/80 to-background/40" />
        </div>

        <div className="relative container mx-auto px-4 h-full flex items-end pb-16">
          <div className="grid md:grid-cols-[300px,1fr] gap-8 w-full">
            {/* Poster */}
            <div className="relative aspect-[2/3] rounded-lg overflow-hidden shadow-2xl">
              <Image
                src={currentMovie.poster}
                alt={currentMovie.title}
                fill
                className="object-cover"
              />
            </div>

            {/* Movie Info */}
            <div className="space-y-6">
              <div>
                <h1 className="text-5xl font-bold mb-2">{currentMovie.title}</h1>
                <div className="flex items-center gap-4 text-muted-foreground">
                  <span>{currentMovie.year}</span>
                  <span className="flex items-center gap-1">
                    <Clock className="h-4 w-4" />
                    {currentMovie.duration}m
                  </span>
                  {currentMovie.rating != null && (
                    <span className="flex items-center gap-1">
                      <Star className="h-4 w-4 fill-yellow-400 text-yellow-400" />
                      {currentMovie.rating.toFixed(1)}
                    </span>
                  )}
                  <span className="px-2 py-1 bg-primary text-primary-foreground text-sm rounded">
                    {currentMovie.quality}
                  </span>
                </div>
              </div>

              {/* Genres */}
              <div className="flex gap-2 flex-wrap">
                {currentMovie.genres?.map((genre) => (
                  <span key={genre} className="px-3 py-1 bg-secondary text-secondary-foreground rounded-full text-sm">
                    {genre}
                  </span>
                ))}
              </div>

              {/* Description */}
              <p className="text-lg leading-relaxed">{currentMovie.description}</p>

              {/* Director */}
              {currentMovie.director && (
                <div>
                  <span className="text-muted-foreground">Director: </span>
                  <span className="font-semibold">{currentMovie.director}</span>
                </div>
              )}

              {/* Action Buttons */}
              <div className="flex gap-4 flex-wrap">
                <Link href={`/watch/${currentMovie.id}`}>
                  <Button size="lg" className="gap-2">
                    <Play className="h-5 w-5" />
                    Watch Now
                  </Button>
                </Link>

                <Button size="lg" variant="outline" className="gap-2" onClick={handleAddToLibrary}>
                  <Plus className="h-5 w-5" />
                  Add to Library
                </Button>

                <Button size="lg" variant="outline" className="gap-2" onClick={handleToggleFavorite}>
                  <Heart className="h-5 w-5" />
                  Favorite
                </Button>

                {currentMovie.downloadUrl && (
                  <a href={currentMovie.downloadUrl} download>
                    <Button size="lg" variant="outline" className="gap-2">
                      <Download className="h-5 w-5" />
                      Download
                    </Button>
                  </a>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="container mx-auto px-4 py-12 space-y-12">
        {/* Cast */}
        {currentMovie.cast && currentMovie.cast.length > 0 && (
          <div className="space-y-4">
            <h2 className="text-2xl font-bold">Cast</h2>
            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
              {currentMovie.cast.map((member) => (
                <div key={member.id} className="text-center space-y-2">
                  {member.photo && (
                    <div className="relative aspect-square rounded-full overflow-hidden mx-auto w-24">
                      <Image
                        src={member.photo}
                        alt={member.name}
                        fill
                        className="object-cover"
                      />
                    </div>
                  )}
                  <div>
                    <p className="font-semibold text-sm">{member.name}</p>
                    <p className="text-xs text-muted-foreground">{member.character}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Torrent Seeding */}
        {currentMovie.torrentMagnet && (
          <TorrentSeedWidget movieId={currentMovie.id} movieTitle={currentMovie.title} />
        )}

        {/* Related Movies */}
        {relatedMovies.length > 0 && (
          <MovieRow title="Related Movies" movies={relatedMovies} />
        )}
      </div>
    </div>
  );
}
