'use client';

import { useEffect, useState } from 'react';
import { TorrentInfo } from '@/types';
import { Button } from '@/components/ui/button';
import { Progress } from '@/components/ui/progress';
import { Play, Pause, Upload, Download, Users } from 'lucide-react';
import { torrentService } from '@/services/torrent.service';
import { useToast } from '@/components/ui/toaster';

interface TorrentSeedWidgetProps {
  movieId: string;
  movieTitle: string;
}

export default function TorrentSeedWidget({ movieId, movieTitle }: TorrentSeedWidgetProps) {
  const [torrentInfo, setTorrentInfo] = useState<TorrentInfo | null>(null);
  const [isSeeding, setIsSeeding] = useState(false);
  const [loading, setLoading] = useState(false);
  const { toast } = useToast();

  useEffect(() => {
    loadTorrentInfo();
    const interval = setInterval(loadTorrentInfo, 5000); // Update every 5 seconds
    return () => clearInterval(interval);
  }, [movieId]);

  const loadTorrentInfo = async () => {
    try {
      const response = await torrentService.getTorrentInfo(movieId);
      if (response.success && response.data) {
        setTorrentInfo(response.data);
        setIsSeeding(response.data.isSeeding);
      }
    } catch (error) {
      // Movie not being seeded
      setTorrentInfo(null);
      setIsSeeding(false);
    }
  };

  const handleStartSeeding = async () => {
    setLoading(true);
    try {
      const response = await torrentService.startSeeding(movieId);
      if (response.success && response.data) {
        setTorrentInfo(response.data);
        setIsSeeding(true);
        toast({
          title: 'Success',
          description: 'Started seeding torrent',
        });
      }
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error.message || 'Failed to start seeding',
      });
    } finally {
      setLoading(false);
    }
  };

  const handleStopSeeding = async () => {
    setLoading(true);
    try {
      await torrentService.stopSeeding(movieId);
      setTorrentInfo(null);
      setIsSeeding(false);
      toast({
        title: 'Success',
        description: 'Stopped seeding torrent',
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error.message || 'Failed to stop seeding',
      });
    } finally {
      setLoading(false);
    }
  };

  const formatBytes = (bytes: number) => {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
  };

  const formatSpeed = (bytesPerSecond: number) => {
    return formatBytes(bytesPerSecond) + '/s';
  };

  return (
    <div className="border rounded-lg p-4 bg-card">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-semibold">Torrent Seeding</h3>
        {isSeeding ? (
          <Button
            variant="destructive"
            size="sm"
            onClick={handleStopSeeding}
            disabled={loading}
          >
            <Pause className="h-4 w-4 mr-2" />
            Stop Seeding
          </Button>
        ) : (
          <Button
            variant="default"
            size="sm"
            onClick={handleStartSeeding}
            disabled={loading}
          >
            <Play className="h-4 w-4 mr-2" />
            Start Seeding
          </Button>
        )}
      </div>

      {isSeeding && torrentInfo ? (
        <div className="space-y-4">
          {/* Status Message */}
          <div className="bg-green-500/10 border border-green-500/20 rounded p-3">
            <p className="text-sm text-green-600 dark:text-green-400">
              You are currently seeding "{movieTitle}"
            </p>
          </div>

          {/* Progress */}
          <div>
            <div className="flex justify-between text-sm mb-1">
              <span>Progress</span>
              <span>{torrentInfo.progress.toFixed(1)}%</span>
            </div>
            <Progress value={torrentInfo.progress} />
          </div>

          {/* Stats Grid */}
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-1">
              <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <Upload className="h-4 w-4" />
                <span>Upload</span>
              </div>
              <div className="text-lg font-semibold">
                {formatSpeed(torrentInfo.uploadSpeed)}
              </div>
              <div className="text-xs text-muted-foreground">
                Total: {formatBytes(torrentInfo.uploaded)}
              </div>
            </div>

            <div className="space-y-1">
              <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <Download className="h-4 w-4" />
                <span>Download</span>
              </div>
              <div className="text-lg font-semibold">
                {formatSpeed(torrentInfo.downloadSpeed)}
              </div>
              <div className="text-xs text-muted-foreground">
                Total: {formatBytes(torrentInfo.downloaded)}
              </div>
            </div>

            <div className="space-y-1">
              <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <Users className="h-4 w-4" />
                <span>Peers</span>
              </div>
              <div className="text-lg font-semibold">{torrentInfo.peers}</div>
            </div>

            <div className="space-y-1">
              <div className="text-sm text-muted-foreground">Ratio</div>
              <div className="text-lg font-semibold">
                {torrentInfo.ratio.toFixed(2)}
              </div>
            </div>
          </div>
        </div>
      ) : (
        <div className="text-center text-muted-foreground py-8">
          <p>Not currently seeding this movie</p>
          <p className="text-sm mt-2">
            Help other users by seeding this content
          </p>
        </div>
      )}
    </div>
  );
}
