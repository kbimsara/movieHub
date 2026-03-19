'use client';

import { useEffect, useRef, useState, useCallback } from 'react';
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
import { Play, Pause, Volume2, VolumeX, Maximize, Minimize, Settings, Subtitles } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Movie } from '@/types';

interface VideoPlayerProps {
  movie: Movie;
  onProgressUpdate?: (progress: number) => void;
}

export default function VideoPlayer({ movie, onProgressUpdate }: VideoPlayerProps) {
  const videoRef = useRef<HTMLVideoElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const seekBarRef = useRef<HTMLDivElement>(null);
  const hlsRef = useRef<Hls | null>(null);
  const dispatch = useAppDispatch();
  const playerState = useAppSelector((state) => state.player);

  const [showControls, setShowControls] = useState(true);
  const [controlsTimeout, setControlsTimeout] = useState<NodeJS.Timeout | null>(null);
  const [buffered, setBuffered] = useState(0);          // 0-100 %
  const [isScrubbing, setIsScrubbing] = useState(false);
  const [scrubPercent, setScrubPercent] = useState(0);  // live drag value
  const [hoverPercent, setHoverPercent] = useState<number | null>(null); // tooltip position
  const [isFullscreen, setIsFullscreen] = useState(false);

  useEffect(() => {
    if (!videoRef.current || !movie.streamUrl) return;

    const video = videoRef.current;
    const src = movie.streamUrl;

    // Destroy any existing HLS instance before re-initialising
    if (hlsRef.current) {
      hlsRef.current.destroy();
      hlsRef.current = null;
    }

    const isHls = src.endsWith('.m3u8') || src.includes('.m3u8?');

    if (isHls && Hls.isSupported()) {
      const hls = new Hls({ enableWorker: true, lowLatencyMode: true });

      hls.loadSource(src);
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
              // Fatal non-recoverable — fall back to native src
              hls.destroy();
              hlsRef.current = null;
              video.src = src;
              break;
          }
        }
      });

      hlsRef.current = hls;

      return () => {
        hls.destroy();
        hlsRef.current = null;
      };
    } else if (isHls && video.canPlayType('application/vnd.apple.mpegurl')) {
      // Safari native HLS
      video.src = src;
      video.play().catch((err) => console.warn('[VideoPlayer] autoplay blocked:', err));
    } else {
      // Plain MP4 / WebM — use native video element with range-request seeking
      video.src = src;
      video.load();
      video.play().catch((err) => console.warn('[VideoPlayer] autoplay blocked:', err));
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

    const handleProgress = () => {
      if (video.buffered.length > 0 && video.duration) {
        const bufferedEnd = video.buffered.end(video.buffered.length - 1);
        setBuffered((bufferedEnd / video.duration) * 100);
      }
    };

    const handlePlay = () => dispatch(setPlaying(true));
    const handlePause = () => dispatch(setPlaying(false));

    video.addEventListener('timeupdate', handleTimeUpdate);
    video.addEventListener('loadedmetadata', handleLoadedMetadata);
    video.addEventListener('progress', handleProgress);
    video.addEventListener('play', handlePlay);
    video.addEventListener('pause', handlePause);

    return () => {
      video.removeEventListener('timeupdate', handleTimeUpdate);
      video.removeEventListener('loadedmetadata', handleLoadedMetadata);
      video.removeEventListener('progress', handleProgress);
      video.removeEventListener('play', handlePlay);
      video.removeEventListener('pause', handlePause);
    };
  }, [dispatch, onProgressUpdate]);

  // ── Scrub bar helpers ──────────────────────────────────────────────────────
  const percentFromEvent = useCallback((e: React.MouseEvent | MouseEvent): number => {
    const bar = seekBarRef.current;
    if (!bar) return 0;
    const rect = bar.getBoundingClientRect();
    return Math.min(100, Math.max(0, ((e.clientX - rect.left) / rect.width) * 100));
  }, []);

  const seekToPercent = useCallback((pct: number) => {
    const video = videoRef.current;
    if (!video || !video.duration) return;
    video.currentTime = (pct / 100) * video.duration;
    dispatch(setCurrentTime(video.currentTime));
  }, [dispatch]);

  const handleSeekBarMouseDown = (e: React.MouseEvent) => {
    const pct = percentFromEvent(e);
    setIsScrubbing(true);
    setScrubPercent(pct);
    videoRef.current?.pause();

    const onMove = (ev: MouseEvent) => {
      const p = percentFromEvent(ev);
      setScrubPercent(p);
    };
    const onUp = (ev: MouseEvent) => {
      const p = percentFromEvent(ev);
      seekToPercent(p);
      setIsScrubbing(false);
      videoRef.current?.play().catch((err) => console.warn('[VideoPlayer] resume after scrub blocked:', err));
      window.removeEventListener('mousemove', onMove);
      window.removeEventListener('mouseup', onUp);
    };
    window.addEventListener('mousemove', onMove);
    window.addEventListener('mouseup', onUp);
  };

  const handleSeekBarClick = (e: React.MouseEvent) => {
    if (!isScrubbing) {
      seekToPercent(percentFromEvent(e));
    }
  };

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
      setIsFullscreen(true);
      dispatch(setFullscreen(true));
    } else {
      document.exitFullscreen();
      setIsFullscreen(false);
      dispatch(setFullscreen(false));
    }
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
        playsInline
        preload="auto"
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

          {/* ── Seek / Scrub Bar ─────────────────────────────────── */}
          <div
            ref={seekBarRef}
            className="group relative h-5 flex items-center cursor-pointer select-none"
            onMouseDown={handleSeekBarMouseDown}
            onClick={handleSeekBarClick}
            onMouseMove={(e) => setHoverPercent(percentFromEvent(e))}
            onMouseLeave={() => setHoverPercent(null)}
          >
            {/* Track background */}
            <div className="absolute inset-y-0 top-1/2 -translate-y-1/2 w-full h-1 group-hover:h-1.5 transition-all rounded-full bg-white/20 overflow-hidden">
              {/* Buffered */}
              <div
                className="absolute left-0 top-0 h-full bg-white/40 rounded-full transition-all"
                style={{ width: `${buffered}%` }}
              />
              {/* Played */}
              <div
                className="absolute left-0 top-0 h-full bg-red-500 rounded-full"
                style={{ width: `${isScrubbing ? scrubPercent : (playerState.currentTime / (playerState.duration || 1)) * 100}%` }}
              />
            </div>

            {/* Thumb */}
            <div
              className="absolute top-1/2 -translate-y-1/2 w-3 h-3 rounded-full bg-red-500 shadow opacity-0 group-hover:opacity-100 transition-opacity -translate-x-1/2 pointer-events-none"
              style={{ left: `${isScrubbing ? scrubPercent : (playerState.currentTime / (playerState.duration || 1)) * 100}%` }}
            />

            {/* Hover tooltip */}
            {hoverPercent !== null && playerState.duration > 0 && (
              <div
                className="absolute bottom-6 -translate-x-1/2 bg-black/80 text-white text-xs px-2 py-0.5 rounded pointer-events-none whitespace-nowrap"
                style={{ left: `${hoverPercent}%` }}
              >
                {formatTime((hoverPercent / 100) * playerState.duration)}
              </div>
            )}
          </div>

          {/* Controls row */}
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
                className="w-20 accent-red-500"
              />

              <span className="text-sm text-white tabular-nums">
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
                {isFullscreen ? <Minimize /> : <Maximize />}
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
