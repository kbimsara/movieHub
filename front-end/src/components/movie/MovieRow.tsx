'use client';

import { useRef } from 'react';
import { Movie } from '@/types';
import MovieCard from './MovieCard';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { Button } from '@/components/ui/button';

interface MovieRowProps {
  title: string;
  movies: Movie[];
  showProgress?: boolean;
}

export default function MovieRow({ title, movies, showProgress = false }: MovieRowProps) {
  const scrollRef = useRef<HTMLDivElement>(null);

  const scroll = (direction: 'left' | 'right') => {
    if (scrollRef.current) {
      const scrollAmount = direction === 'left' ? -800 : 800;
      scrollRef.current.scrollBy({ left: scrollAmount, behavior: 'smooth' });
    }
  };

  if (movies.length === 0) {
    return null;
  }

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-bold">{title}</h2>
      
      <div className="relative group/row">
        {/* Scroll Left Button */}
        <Button
          variant="ghost"
          size="icon"
          className="absolute left-0 top-1/2 -translate-y-1/2 z-10 opacity-0 group-hover/row:opacity-100 transition-opacity bg-black/50 hover:bg-black/70"
          onClick={() => scroll('left')}
        >
          <ChevronLeft className="h-6 w-6" />
        </Button>

        {/* Movies Scroll Container */}
        <div
          ref={scrollRef}
          className="flex gap-4 overflow-x-auto scrollbar-hide scroll-smooth"
          style={{ scrollbarWidth: 'none', msOverflowStyle: 'none' }}
        >
          {movies.map((movie) => (
            <div key={movie.id} className="flex-shrink-0 w-48">
              <MovieCard movie={movie} showProgress={showProgress} />
            </div>
          ))}
        </div>

        {/* Scroll Right Button */}
        <Button
          variant="ghost"
          size="icon"
          className="absolute right-0 top-1/2 -translate-y-1/2 z-10 opacity-0 group-hover/row:opacity-100 transition-opacity bg-black/50 hover:bg-black/70"
          onClick={() => scroll('right')}
        >
          <ChevronRight className="h-6 w-6" />
        </Button>
      </div>
    </div>
  );
}
