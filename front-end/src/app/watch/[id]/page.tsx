'use client';

import { useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { useMovies } from '@/hooks/useMovies';
import { useLibrary } from '@/hooks/useLibrary';
import VideoPlayer from '@/components/player/VideoPlayer';
import { ArrowLeft } from 'lucide-react';
import { Button } from '@/components/ui/button';

export default function WatchPage() {
  const params = useParams();
  const router = useRouter();
  const movieId = params.id as string;
  const { currentMovie, fetchMovieById } = useMovies();
  const { updateProgress } = useLibrary();

  useEffect(() => {
    if (movieId) {
      fetchMovieById(movieId);
    }
  }, [movieId]);

  const handleProgressUpdate = (progress: number) => {
    if (currentMovie) {
      updateProgress(currentMovie.id, progress);
    }
  };

  if (!currentMovie) {
    return (
      <div className="flex items-center justify-center h-screen">
        <p>Loading...</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-black">
      {/* Back Button */}
      <div className="absolute top-20 left-4 z-50">
        <Button
          variant="ghost"
          size="icon"
          onClick={() => router.back()}
          className="bg-black/50 hover:bg-black/70"
        >
          <ArrowLeft className="h-6 w-6" />
        </Button>
      </div>

      {/* Video Player */}
      <div className="w-full h-screen flex items-center justify-center">
        <div className="w-full max-w-screen-2xl">
          <VideoPlayer movie={currentMovie} onProgressUpdate={handleProgressUpdate} />
        </div>
      </div>

      {/* Movie Info Below Player */}
      <div className="container mx-auto px-4 py-8 text-white">
        <h1 className="text-3xl font-bold mb-2">{currentMovie.title}</h1>
        <p className="text-gray-400 mb-4">{currentMovie.description}</p>
        <div className="flex gap-2">
          {currentMovie.genres?.map((genre) => (
            <span key={genre} className="px-3 py-1 bg-gray-800 rounded-full text-sm">
              {genre}
            </span>
          ))}
        </div>
      </div>
    </div>
  );
}
