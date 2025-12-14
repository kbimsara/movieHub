'use client';

import { useEffect, useRef, useState } from 'react';
import Hls from 'hls.js';
import { useAppDispatch, useAppSelector } from '@/hooks/redux';
import {
  setPlaying,
  setCurrentTime,
  setDuration,
  setVolume,
  setMuted,
  setQuality,
  setSubtitle,
  setFullscreen,
} from '@/store/slices/playerSlice';
import { Play, Pause, Volume2, VolumeX, Maximize, Settings, Subtitles } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Progress } from '@/components/ui/progress';
import { Movie } from '@/types';

interface VideoPlayerProps {
  movie: Movie;
  onProgressUpdate?: (progress: number) => void;
}

export default function VideoPlayer({ movie, onProgressUpdate }: VideoPlayerProps) {
  const videoRef = useRef<HTMLVideoElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const hlsRef = useRef<Hls | null>(null);
  const dispatch = useAppDispatch();
  const playerState = useAppSelector((state) => state.player);

  const [showControls, setShowControls] = useState(true);
  const [controlsTimeout, setControlsTimeout] = useState<NodeJS.Timeout | null>(null);

  useEffect(() => {
    if (!videoRef.current || !movie.streamUrl) return;

    const video = videoRef.current;

    if (Hls.isSupported()) {
      const hls = new Hls({
        enableWorker: true,
        lowLatencyMode: true,
      });

      hls.loadSource(movie.streamUrl);
      hls.attachMedia(video);

      hls.on(Hls.Events.MANIFEST_PARSED, () => {
        console.log('HLS manifest loaded');
      });

      hls.on(Hls.Events.ERROR, (event, data) => {
        console.error('HLS error:', data);
        if (data.fatal) {
          switch (data.type) {
            case Hls.ErrorTypes.NETWORK_ERROR:
              hls.startLoad();
              break;
            case Hls.ErrorTypes.MEDIA_ERROR:
              hls.recoverMediaError();
              break;
            default:
              hls.destroy();
              break;
          }
        }
      });

      hlsRef.current = hls;

      return () => {
        hls.destroy();
      };
    } else if (video.canPlayType('application/vnd.apple.mpegurl')) {
      video.src = movie.streamUrl;
    }
  }, [movie.streamUrl]);

  // Update progress
  useEffect(() => {
    const video = videoRef.current;
    if (!video) return;

    const handleTimeUpdate = () => {
      dispatch(setCurrentTime(video.currentTime));
      if (onProgressUpdate && video.duration) {
        const progress = (video.currentTime / video.duration) * 100;
        onProgressUpdate(progress);
      }
    };

    const handleLoadedMetadata = () => {
      dispatch(setDuration(video.duration));
    };

    video.addEventListener('timeupdate', handleTimeUpdate);
    video.addEventListener('loadedmetadata', handleLoadedMetadata);

    return () => {
      video.removeEventListener('timeupdate', handleTimeUpdate);
      video.removeEventListener('loadedmetadata', handleLoadedMetadata);
    };
  }, [dispatch, onProgressUpdate]);

  // Auto-hide controls
  const resetControlsTimeout = () => {
    if (controlsTimeout) clearTimeout(controlsTimeout);
    setShowControls(true);
    const timeout = setTimeout(() => setShowControls(false), 3000);
    setControlsTimeout(timeout);
  };

  const togglePlay = () => {
    const video = videoRef.current;
    if (!video) return;

    if (video.paused) {
      video.play();
      dispatch(setPlaying(true));
    } else {
      video.pause();
      dispatch(setPlaying(false));
    }
  };

  const toggleMute = () => {
    const video = videoRef.current;
    if (!video) return;

    video.muted = !video.muted;
    dispatch(setMuted(video.muted));
  };

  const handleVolumeChange = (value: number) => {
    const video = videoRef.current;
    if (!video) return;

    video.volume = value;
    dispatch(setVolume(value));
  };

  const toggleFullscreen = () => {
    const container = containerRef.current;
    if (!container) return;

    if (!document.fullscreenElement) {
      container.requestFullscreen();
      dispatch(setFullscreen(true));
    } else {
      document.exitFullscreen();
      dispatch(setFullscreen(false));
    }
  };

  const handleSeek = (percent: number) => {
    const video = videoRef.current;
    if (!video || !video.duration) return;

    video.currentTime = (percent / 100) * video.duration;
  };

  const formatTime = (seconds: number) => {
    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const s = Math.floor(seconds % 60);
    return h > 0
      ? `${h}:${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`
      : `${m}:${s.toString().padStart(2, '0')}`;
  };

  return (
    <div
      ref={containerRef}
      className="relative w-full bg-black aspect-video"
      onMouseMove={resetControlsTimeout}
      onMouseLeave={() => setShowControls(false)}
    >
      <video
        ref={videoRef}
        className="w-full h-full"
        onClick={togglePlay}
      />

      {/* Controls Overlay */}
      <div
        className={`absolute inset-0 bg-gradient-to-t from-black/80 via-transparent to-transparent transition-opacity ${
          showControls ? 'opacity-100' : 'opacity-0'
        }`}
      >
        {/* Play/Pause Center Button */}
        <div className="absolute inset-0 flex items-center justify-center">
          <Button
            variant="ghost"
            size="icon"
            className="w-16 h-16 rounded-full bg-black/50 hover:bg-black/70"
            onClick={togglePlay}
          >
            {playerState.isPlaying ? (
              <Pause className="h-8 w-8" />
            ) : (
              <Play className="h-8 w-8" />
            )}
          </Button>
        </div>

        {/* Bottom Controls */}
        <div className="absolute bottom-0 left-0 right-0 p-4 space-y-2">
          {/* Progress Bar */}
          <div className="cursor-pointer" onClick={(e) => {
            const rect = e.currentTarget.getBoundingClientRect();
            const percent = ((e.clientX - rect.left) / rect.width) * 100;
            handleSeek(percent);
          }}>
            <Progress
              value={(playerState.currentTime / playerState.duration) * 100}
              className="h-1 cursor-pointer"
            />
          </div>

          {/* Controls */}
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2">
              <Button variant="ghost" size="icon" onClick={togglePlay}>
                {playerState.isPlaying ? <Pause /> : <Play />}
              </Button>

              <Button variant="ghost" size="icon" onClick={toggleMute}>
                {playerState.isMuted ? <VolumeX /> : <Volume2 />}
              </Button>

              <input
                type="range"
                min="0"
                max="1"
                step="0.01"
                value={playerState.volume}
                onChange={(e) => handleVolumeChange(parseFloat(e.target.value))}
                className="w-20"
              />

              <span className="text-sm text-white">
                {formatTime(playerState.currentTime)} / {formatTime(playerState.duration)}
              </span>
            </div>

            <div className="flex items-center gap-2">
              <Button variant="ghost" size="icon">
                <Subtitles />
              </Button>

              <Button variant="ghost" size="icon">
                <Settings />
              </Button>

              <Button variant="ghost" size="icon" onClick={toggleFullscreen}>
                <Maximize />
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
