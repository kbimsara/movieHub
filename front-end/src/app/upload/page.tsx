'use client';

import { useState, useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { uploadService } from '@/services/upload.service';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Progress } from '@/components/ui/progress';
import { useToast } from '@/components/ui/toaster';
import { Upload, FileVideo, X, Image as ImageIcon, Film, Info, Star, CheckCircle2, Loader2, AlertCircle } from 'lucide-react';
import { Card } from '@/components/ui/card';

const movieMetadataSchema = z.object({
  title: z.string().min(1, 'Title is required'),
  description: z.string().min(10, 'Description must be at least 10 characters'),
  year: z.number().min(1900).max(new Date().getFullYear() + 1),
  duration: z.number().min(1, 'Duration is required'),
  genres: z.string().min(1, 'At least one genre is required'),
  quality: z.enum(['480p', '720p', '1080p', '4K']),
  rating: z.number().min(0).max(10).optional(),
  tags: z.string().optional(),
  cast: z.string().optional(),
  director: z.string().optional(),
  trailer: z.string().url().optional().or(z.literal('')),
});

type MovieMetadataForm = z.infer<typeof movieMetadataSchema>;

interface DetectedMeta {
  durationMinutes: number;
  quality: '480p' | '720p' | '1080p' | '4K';
  width: number;
  height: number;
}

export default function UploadPage() {
  const router = useRouter();
  const { toast } = useToast();
  const [uploadFile, setUploadFile] = useState<File | null>(null);
  const [posterFile, setPosterFile] = useState<File | null>(null);
  const [posterPreview, setPosterPreview] = useState<string | null>(null);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadStatus, setUploadStatus] = useState<'idle' | 'uploading' | 'processing' | 'ready' | 'failed'>('idle');
  const [isAnalyzing, setIsAnalyzing] = useState(false);
  const [detectedMeta, setDetectedMeta] = useState<DetectedMeta | null>(null);
  const [detectionFailed, setDetectionFailed] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
    setValue,
  } = useForm<MovieMetadataForm>({
    resolver: zodResolver(movieMetadataSchema),
    defaultValues: {
      quality: '1080p',
      year: new Date().getFullYear(),
    },
  });

  // Detect duration and resolution from the video file using the browser's video element.
  // Resolves with detected values when successful, or detected=false when the browser
  // can't read the file (e.g. MKV in Safari) or the 10-second timeout fires.
  const detectVideoMetadata = useCallback(
    (file: File): Promise<{ detected: false } | ({ detected: true } & DetectedMeta)> =>
      new Promise((resolve) => {
        const url = URL.createObjectURL(file);
        const video = document.createElement('video');
        video.preload = 'metadata';
        video.muted = true;

        const cleanup = () => URL.revokeObjectURL(url);

        // Fallback if metadata never loads (unsupported codec / slow disk)
        const timeout = setTimeout(() => {
          cleanup();
          resolve({ detected: false });
        }, 10_000);

        video.onloadedmetadata = () => {
          clearTimeout(timeout);
          cleanup();

          const rawSeconds = video.duration;
          const durationMinutes = Math.max(1, Math.round(rawSeconds / 60));
          const height = video.videoHeight;
          const width = video.videoWidth;

          if (height === 0 || rawSeconds === 0 || !isFinite(rawSeconds)) {
            resolve({ detected: false });
            return;
          }

          const quality: '480p' | '720p' | '1080p' | '4K' =
            height >= 2160 ? '4K' :
            height >= 1080 ? '1080p' :
            height >= 720  ? '720p'  : '480p';

          resolve({ detected: true, durationMinutes, quality, width, height });
        };

        video.onerror = () => {
          clearTimeout(timeout);
          cleanup();
          resolve({ detected: false });
        };

        video.src = url;
      }),
    []
  );

  const onDrop = useCallback(
    async (acceptedFiles: File[]) => {
      if (acceptedFiles.length === 0) return;

      const file = acceptedFiles[0];
      setUploadFile(file);
      setDetectedMeta(null);
      setDetectionFailed(false);
      setIsAnalyzing(true);

      try {
        const result = await detectVideoMetadata(file);

        if (result.detected) {
          setValue('duration', result.durationMinutes);
          setValue('quality', result.quality);
          setDetectedMeta(result);
          toast({
            title: 'File ready',
            description: `${result.durationMinutes} min · ${result.width}×${result.height} (${result.quality}) · ${(file.size / 1024 / 1024).toFixed(1)} MB`,
          });
        } else {
          setDetectionFailed(true);
          toast({
            title: 'File selected',
            description: `${file.name} · ${(file.size / 1024 / 1024).toFixed(1)} MB — please fill in duration and quality manually`,
          });
        }
      } finally {
        setIsAnalyzing(false);
      }
    },
    [detectVideoMetadata, setValue, toast]
  );

  const onPosterDrop = useCallback((acceptedFiles: File[]) => {
    if (acceptedFiles.length > 0) {
      const file = acceptedFiles[0];
      setPosterFile(file);

      const reader = new FileReader();
      reader.onloadend = () => {
        setPosterPreview(reader.result as string);
      };
      reader.readAsDataURL(file);

      toast({
        title: 'Poster selected',
        description: file.name,
      });
    }
  }, [toast]);

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: {
      'video/*': ['.mp4', '.mkv', '.avi', '.mov', '.wmv'],
    },
    maxFiles: 1,
    disabled: isUploading,
  });

  const { getRootProps: getPosterRootProps, getInputProps: getPosterInputProps, isDragActive: isPosterDragActive } = useDropzone({
    onDrop: onPosterDrop,
    accept: {
      'image/*': ['.jpg', '.jpeg', '.png', '.webp'],
    },
    maxFiles: 1,
    disabled: isUploading,
  });

  const onSubmit = async (data: MovieMetadataForm) => {
    if (!uploadFile) {
      toast({ title: 'Error', description: 'Please select a video file' });
      return;
    }

    setIsUploading(true);
    setUploadStatus('uploading');

    try {
      const metadata = {
        title: data.title,
        description: data.description,
        year: data.year,
        duration: data.duration,
        genres: data.genres.split(',').map((g) => g.trim()),
        quality: data.quality,
        rating: data.rating || 0,
        tags: data.tags ? data.tags.split(',').map((t) => t.trim()) : [],
        cast: data.cast ? data.cast.split(',').map((c) => c.trim()) : [],
        director: data.director,
        trailer: data.trailer || undefined,
        poster: posterFile ?? undefined,
      };

      const response = await uploadService.uploadMovie(
        uploadFile,
        metadata,
        (progress) => setUploadProgress(progress)
      );

      if (response.success) {
        setUploadStatus('processing');
        toast({ title: 'Upload complete', description: 'Your movie is being processed' });

        const movieId = response.data?.movieId;

        setTimeout(() => {
          setUploadStatus('ready');
          toast({ title: 'Success!', description: 'Your movie is now live!' });
          setTimeout(() => {
            router.push(movieId ? `/movie/${movieId}` : '/');
          }, 2000);
        }, 3000);
      }
    } catch (error: any) {
      setUploadStatus('failed');
      setUploadProgress(0);
      setIsUploading(false);

      if (error.response?.status === 404) {
        toast({
          title: 'Backend not ready',
          description: 'The upload API endpoint is not yet implemented.',
        });
      } else {
        toast({
          title: 'Upload failed',
          description: error.message || 'An error occurred during upload',
        });
      }
    }
  };

  const removeFile = () => {
    setUploadFile(null);
    setUploadProgress(0);
    setDetectedMeta(null);
    setDetectionFailed(false);
    // Reset auto-detected fields back to defaults
    setValue('duration', undefined as unknown as number);
    setValue('quality', '1080p');
  };

  const removePoster = () => {
    setPosterFile(null);
    setPosterPreview(null);
  };

  return (
    <div className="container mx-auto px-4 py-8 max-w-6xl">
      <div className="mb-8">
        <h1 className="text-4xl font-bold mb-2">Upload Movie</h1>
        <p className="text-muted-foreground">Add a new movie to the catalog with metadata and media files</p>
      </div>

      {/* Public Upload Notice */}
      <div className="mb-6 p-4 bg-blue-500/10 border border-blue-500/20 rounded-lg flex items-start gap-3">
        <Info className="h-5 w-5 text-blue-500 mt-0.5 flex-shrink-0" />
        <div className="text-sm">
          <p className="font-medium text-blue-500 mb-1">Your upload will be public</p>
          <p className="text-muted-foreground">
            Movies you upload will be visible to all users on the home page and can be watched by everyone in the community.
          </p>
        </div>
      </div>

      <div className="grid lg:grid-cols-3 gap-8">
        {/* Main Upload Section */}
        <div className="lg:col-span-2 space-y-6">
          {/* Video File Upload */}
          <Card className="p-6">
            <div className="flex items-center gap-2 mb-4">
              <Film className="h-5 w-5" />
              <h2 className="text-xl font-semibold">Movie File</h2>
            </div>

            {!uploadFile ? (
              <div {...getRootProps()} className="cursor-pointer">
                <input {...getInputProps()} />
                <div className={`border-2 border-dashed rounded-lg p-12 transition-colors ${isDragActive ? 'border-primary bg-primary/5' : 'border-muted-foreground/25'}`}>
                  <div className="flex flex-col items-center justify-center space-y-4">
                    <Upload className="h-16 w-16 text-muted-foreground" />
                    <div className="text-center">
                      <p className="text-lg font-medium">
                        {isDragActive ? 'Drop the video file here' : 'Drag & drop a video file here'}
                      </p>
                      <p className="text-sm text-muted-foreground mt-2">
                        or click to browse (MP4, MKV, AVI, MOV, WMV)
                      </p>
                      <p className="text-xs text-muted-foreground mt-1">Maximum file size: 10 GB</p>
                    </div>
                  </div>
                </div>
              </div>
            ) : (
              <div className="space-y-3">
                <div className="flex items-center justify-between p-4 bg-secondary rounded-lg">
                  <div className="flex items-center gap-4">
                    {isAnalyzing ? (
                      <Loader2 className="h-8 w-8 animate-spin text-primary flex-shrink-0" />
                    ) : (
                      <FileVideo className="h-8 w-8 flex-shrink-0" />
                    )}
                    <div className="min-w-0">
                      <p className="font-medium truncate">{uploadFile.name}</p>
                      <p className="text-sm text-muted-foreground">
                        {(uploadFile.size / 1024 / 1024).toFixed(1)} MB
                        {isAnalyzing && <span className="ml-2 text-primary animate-pulse">Detecting video details…</span>}
                      </p>
                    </div>
                  </div>
                  {!isUploading && !isAnalyzing && (
                    <Button variant="ghost" size="icon" onClick={removeFile} className="flex-shrink-0">
                      <X className="h-5 w-5" />
                    </Button>
                  )}
                </div>

                {/* Detection result banner */}
                {!isAnalyzing && detectedMeta && (
                  <div className="flex items-center gap-2 px-3 py-2 rounded-md bg-green-500/10 border border-green-500/20 text-sm text-green-600">
                    <CheckCircle2 className="h-4 w-4 flex-shrink-0" />
                    <span>
                      Auto-detected: <strong>{detectedMeta.durationMinutes} min</strong> · <strong>{detectedMeta.width}×{detectedMeta.height}</strong> ({detectedMeta.quality})
                    </span>
                  </div>
                )}

                {!isAnalyzing && detectionFailed && (
                  <div className="flex items-center gap-2 px-3 py-2 rounded-md bg-yellow-500/10 border border-yellow-500/20 text-sm text-yellow-600">
                    <AlertCircle className="h-4 w-4 flex-shrink-0" />
                    <span>Could not read video metadata — please fill in duration and quality manually.</span>
                  </div>
                )}

                {/* Upload Progress */}
                {isUploading && (
                  <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                      <span className="text-muted-foreground">
                        {uploadStatus === 'uploading' && 'Uploading…'}
                        {uploadStatus === 'processing' && 'Processing…'}
                        {uploadStatus === 'ready' && 'Complete!'}
                        {uploadStatus === 'failed' && 'Failed'}
                      </span>
                      <span className="font-medium">{uploadProgress}%</span>
                    </div>
                    <Progress value={uploadProgress} />
                  </div>
                )}
              </div>
            )}
          </Card>

          {/* Metadata Form */}
          <Card className="p-6">
            <div className="flex items-center gap-2 mb-4">
              <Info className="h-5 w-5" />
              <h2 className="text-xl font-semibold">Movie Information</h2>
            </div>

            <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
              <div className="space-y-2">
                <Label htmlFor="title">Title *</Label>
                <Input id="title" placeholder="Enter movie title" {...register('title')} />
                {errors.title && <p className="text-sm text-destructive">{errors.title.message}</p>}
              </div>

              <div className="space-y-2">
                <Label htmlFor="description">Description *</Label>
                <textarea
                  id="description"
                  placeholder="Enter movie description..."
                  className="flex min-h-[120px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                  {...register('description')}
                />
                {errors.description && <p className="text-sm text-destructive">{errors.description.message}</p>}
              </div>

              <div className="grid sm:grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="year">Year *</Label>
                  <Input
                    id="year"
                    type="number"
                    placeholder="2024"
                    {...register('year', { valueAsNumber: true })}
                  />
                  {errors.year && <p className="text-sm text-destructive">{errors.year.message}</p>}
                </div>

                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <Label htmlFor="duration">Duration (minutes) *</Label>
                    {isAnalyzing && <Loader2 className="h-3 w-3 animate-spin text-primary" />}
                    {!isAnalyzing && detectedMeta && (
                      <span className="text-xs text-green-500 font-medium flex items-center gap-1">
                        <CheckCircle2 className="h-3 w-3" /> auto-detected
                      </span>
                    )}
                  </div>
                  <Input
                    id="duration"
                    type="number"
                    placeholder="120"
                    className={detectedMeta ? 'border-green-500/50 focus-visible:ring-green-500/30' : ''}
                    {...register('duration', { valueAsNumber: true })}
                  />
                  {errors.duration && <p className="text-sm text-destructive">{errors.duration.message}</p>}
                </div>

                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <Label htmlFor="quality">Quality *</Label>
                    {isAnalyzing && <Loader2 className="h-3 w-3 animate-spin text-primary" />}
                    {!isAnalyzing && detectedMeta && (
                      <span className="text-xs text-green-500 font-medium flex items-center gap-1">
                        <CheckCircle2 className="h-3 w-3" /> auto-detected
                      </span>
                    )}
                  </div>
                  <select
                    id="quality"
                    className={`flex h-10 w-full rounded-md border bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 ${
                      detectedMeta ? 'border-green-500/50 focus-visible:ring-green-500/30' : 'border-input'
                    }`}
                    {...register('quality')}
                  >
                    <option value="480p">480p</option>
                    <option value="720p">720p</option>
                    <option value="1080p">1080p</option>
                    <option value="4K">4K</option>
                  </select>
                  {errors.quality && <p className="text-sm text-destructive">{errors.quality.message}</p>}
                </div>

                <div className="space-y-2">
                  <Label htmlFor="rating">Rating (0–10)</Label>
                  <Input
                    id="rating"
                    type="number"
                    step="0.1"
                    min="0"
                    max="10"
                    placeholder="7.5"
                    {...register('rating', { valueAsNumber: true })}
                  />
                  {errors.rating && <p className="text-sm text-destructive">{errors.rating.message}</p>}
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="genres">Genres (comma-separated) *</Label>
                <Input id="genres" placeholder="Action, Thriller, Drama" {...register('genres')} />
                {errors.genres && <p className="text-sm text-destructive">{errors.genres.message}</p>}
              </div>

              <div className="space-y-2">
                <Label htmlFor="director">Director</Label>
                <Input id="director" placeholder="Director name" {...register('director')} />
              </div>

              <div className="space-y-2">
                <Label htmlFor="cast">Cast (comma-separated)</Label>
                <Input id="cast" placeholder="Actor 1, Actor 2, Actor 3" {...register('cast')} />
              </div>

              <div className="space-y-2">
                <Label htmlFor="tags">Tags (comma-separated)</Label>
                <Input id="tags" placeholder="tag1, tag2, tag3" {...register('tags')} />
              </div>

              <div className="space-y-2">
                <Label htmlFor="trailer">Trailer URL (YouTube, Vimeo, etc.)</Label>
                <Input id="trailer" type="url" placeholder="https://youtube.com/watch?v=..." {...register('trailer')} />
                {errors.trailer && <p className="text-sm text-destructive">{errors.trailer.message}</p>}
              </div>

              <div className="flex gap-4 pt-4">
                <Button type="submit" disabled={!uploadFile || isUploading || isAnalyzing} className="flex-1">
                  {isAnalyzing ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Analyzing…
                    </>
                  ) : isUploading ? (
                    uploadStatus === 'uploading' ? 'Uploading…' : 'Processing…'
                  ) : (
                    <>
                      <Upload className="mr-2 h-4 w-4" />
                      Upload Movie
                    </>
                  )}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => router.back()}
                  disabled={isUploading}
                >
                  Cancel
                </Button>
              </div>
            </form>
          </Card>
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          {/* Poster Upload */}
          <Card className="p-6">
            <div className="flex items-center gap-2 mb-4">
              <ImageIcon className="h-5 w-5" />
              <h2 className="text-xl font-semibold">Poster Image</h2>
            </div>

            {!posterPreview ? (
              <div {...getPosterRootProps()} className="cursor-pointer">
                <input {...getPosterInputProps()} />
                <div className={`border-2 border-dashed rounded-lg p-6 transition-colors ${isPosterDragActive ? 'border-primary bg-primary/5' : 'border-muted-foreground/25'}`}>
                  <div className="flex flex-col items-center justify-center space-y-3">
                    <ImageIcon className="h-12 w-12 text-muted-foreground" />
                    <div className="text-center">
                      <p className="text-sm font-medium">
                        {isPosterDragActive ? 'Drop here' : 'Upload Poster'}
                      </p>
                      <p className="text-xs text-muted-foreground mt-1">JPG, PNG, WebP</p>
                    </div>
                  </div>
                </div>
              </div>
            ) : (
              <div className="space-y-3">
                <div className="relative aspect-[2/3] rounded-lg overflow-hidden">
                  <img
                    src={posterPreview}
                    alt="Poster preview"
                    className="w-full h-full object-cover"
                  />
                </div>
                {!isUploading && (
                  <Button variant="outline" size="sm" onClick={removePoster} className="w-full">
                    <X className="mr-2 h-4 w-4" />
                    Remove Poster
                  </Button>
                )}
              </div>
            )}
          </Card>

          {/* Upload Checklist */}
          <Card className="p-6">
            <h3 className="font-semibold mb-4">Upload Checklist</h3>
            <div className="space-y-3 text-sm">
              <div className="flex items-center gap-2">
                {uploadFile ? (
                  <CheckCircle2 className="h-4 w-4 text-green-500" />
                ) : (
                  <div className="h-4 w-4 rounded-full border-2" />
                )}
                <span className={uploadFile ? 'text-green-500' : 'text-muted-foreground'}>
                  Video file selected
                </span>
              </div>
              <div className="flex items-center gap-2">
                {posterFile ? (
                  <CheckCircle2 className="h-4 w-4 text-green-500" />
                ) : (
                  <div className="h-4 w-4 rounded-full border-2" />
                )}
                <span className={posterFile ? 'text-green-500' : 'text-muted-foreground'}>
                  Poster image added
                </span>
              </div>
              <div className="flex items-center gap-2">
                {watch('title') ? (
                  <CheckCircle2 className="h-4 w-4 text-green-500" />
                ) : (
                  <div className="h-4 w-4 rounded-full border-2" />
                )}
                <span className={watch('title') ? 'text-green-500' : 'text-muted-foreground'}>
                  Title provided
                </span>
              </div>
              <div className="flex items-center gap-2">
                {(watch('description')?.length ?? 0) >= 10 ? (
                  <CheckCircle2 className="h-4 w-4 text-green-500" />
                ) : (
                  <div className="h-4 w-4 rounded-full border-2" />
                )}
                <span className={(watch('description')?.length ?? 0) >= 10 ? 'text-green-500' : 'text-muted-foreground'}>
                  Description added
                </span>
              </div>
              <div className="flex items-center gap-2">
                {watch('genres') ? (
                  <CheckCircle2 className="h-4 w-4 text-green-500" />
                ) : (
                  <div className="h-4 w-4 rounded-full border-2" />
                )}
                <span className={watch('genres') ? 'text-green-500' : 'text-muted-foreground'}>
                  Genres specified
                </span>
              </div>
              <div className="flex items-center gap-2">
                {watch('duration') > 0 ? (
                  <CheckCircle2 className="h-4 w-4 text-green-500" />
                ) : (
                  <div className="h-4 w-4 rounded-full border-2" />
                )}
                <span className={watch('duration') > 0 ? 'text-green-500' : 'text-muted-foreground'}>
                  Duration set{detectedMeta ? ' (auto)' : ''}
                </span>
              </div>
            </div>
          </Card>

          {/* Tips */}
          <Card className="p-6 bg-muted/50">
            <h3 className="font-semibold mb-3 flex items-center gap-2">
              <Star className="h-4 w-4" />
              Upload Tips
            </h3>
            <ul className="space-y-2 text-sm text-muted-foreground">
              <li>• Duration and quality are auto-detected from MP4, MOV, and WebM files</li>
              <li>• MKV / AVI may require manual entry in some browsers</li>
              <li>• Use high-quality posters (recommended: 2000×3000 px)</li>
              <li>• Provide detailed descriptions for better discoverability</li>
              <li>• Add multiple genres and tags</li>
              <li>• Include a trailer URL if available</li>
            </ul>
          </Card>
        </div>
      </div>
    </div>
  );
}
