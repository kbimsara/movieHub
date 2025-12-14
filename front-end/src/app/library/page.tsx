'use client';

import { useEffect } from 'react';
import { useLibrary } from '@/hooks/useLibrary';
import MovieGrid from '@/components/movie/MovieGrid';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';

export default function LibraryPage() {
  const { library, continueWatching, favorites, isLoading, fetchLibrary, fetchContinueWatching, fetchFavorites } = useLibrary();

  useEffect(() => {
    fetchLibrary();
    fetchContinueWatching();
    fetchFavorites();
  }, []);

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold mb-8">My Library</h1>

      <Tabs defaultValue="all" className="space-y-8">
        <TabsList>
          <TabsTrigger value="all">All Movies</TabsTrigger>
          <TabsTrigger value="continue">Continue Watching</TabsTrigger>
          <TabsTrigger value="favorites">Favorites</TabsTrigger>
        </TabsList>

        <TabsContent value="all">
          <MovieGrid
            movies={library.map((item) => item.movie)}
            isLoading={isLoading}
            showProgress
          />
        </TabsContent>

        <TabsContent value="continue">
          <MovieGrid
            movies={continueWatching.map((item) => item.movie)}
            isLoading={isLoading}
            showProgress
          />
        </TabsContent>

        <TabsContent value="favorites">
          <MovieGrid
            movies={favorites.map((item) => item.movie)}
            isLoading={isLoading}
          />
        </TabsContent>
      </Tabs>
    </div>
  );
}
