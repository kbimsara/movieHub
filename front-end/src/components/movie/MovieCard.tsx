'use client';

import Image from 'next/image';
import Link from 'next/link';
import { Movie } from '@/types';
import { Play, Plus, Star, Clock } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { useLibrary } from '@/hooks/useLibrary';
import { useToast } from '@/components/ui/toaster';

interface MovieCardProps {
  movie: Movie;
  showProgress?: boolean;
  progress?: number;
}

export default function MovieCard({ movie, showProgress = false, progress = 0 }: MovieCardProps) {
  const { addToLibrary } = useLibrary();
  const { toast } = useToast();

  const handleAddToLibrary = async (e: React.MouseEvent) => {
    e.preventDefault();
    const result = await addToLibrary(movie.id);
    if (result.success) {
      toast({
        title: 'Success',
        description: 'Movie added to library',
      });
    } else {
      toast({
        title: 'Error',
        description: result.error || 'Failed to add movie',
      });
    }
  };

  return (
    <Link href={`/movie/${movie.id}`}>
      <div className="group relative overflow-hidden rounded-lg bg-card transition-all hover:scale-105 hover:shadow-lg">
        {/* Poster Image */}
        <div className="relative aspect-[2/3] overflow-hidden">
          <Image
            src={movie.poster || '/placeholder.jpg'}
            alt={movie.title}
            fill
            className="object-cover transition-transform group-hover:scale-110"
          />
          
          {/* Overlay on hover */}
          <div className="absolute inset-0 bg-black/60 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
            <Button size="icon" variant="default" className="rounded-full">
              <Play className="h-5 w-5" />
            </Button>
          </div>

          {/* Quality Badge */}
          <div className="absolute top-2 left-2 bg-primary text-primary-foreground text-xs px-2 py-1 rounded">
            {movie.quality}
          </div>

          {/* Rating */}
          <div className="absolute top-2 right-2 bg-black/70 text-white text-xs px-2 py-1 rounded flex items-center gap-1">
            <Star className="h-3 w-3 fill-yellow-400 text-yellow-400" />
            {movie.rating.toFixed(1)}
          </div>
        </div>

        {/* Movie Info */}
        <div className="p-3">
          <h3 className="font-semibold text-sm line-clamp-1 mb-1">{movie.title}</h3>
          
          <div className="flex items-center justify-between text-xs text-muted-foreground mb-2">
            <span>{movie.year}</span>
            <span className="flex items-center gap-1">
              <Clock className="h-3 w-3" />
              {movie.duration}m
            </span>
          </div>

          {/* Genres */}
          <div className="flex gap-1 flex-wrap mb-2">
            {movie.genres.slice(0, 2).map((genre) => (
              <span key={genre} className="text-xs bg-secondary px-2 py-0.5 rounded">
                {genre}
              </span>
            ))}
          </div>

          {/* Progress Bar */}
          {showProgress && progress > 0 && (
            <div className="w-full bg-secondary rounded-full h-1.5 mb-2">
              <div
                className="bg-primary h-1.5 rounded-full transition-all"
                style={{ width: `${progress}%` }}
              />
            </div>
          )}

          {/* Add to Library Button */}
          <Button
            size="sm"
            variant="outline"
            className="w-full opacity-0 group-hover:opacity-100 transition-opacity"
            onClick={handleAddToLibrary}
          >
            <Plus className="h-4 w-4 mr-1" />
            Add to Library
          </Button>
        </div>
      </div>
    </Link>
  );
}
