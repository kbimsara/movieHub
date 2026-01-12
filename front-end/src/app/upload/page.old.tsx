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
import { Upload, FileVideo, X, Image as ImageIcon, Film, Info, Star } from 'lucide-react';
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

export default function UploadPage() {
  const router = useRouter();
  const { toast } = useToast();
  const [uploadFile, setUploadFile] = useState<File | null>(null);
  const [posterFile, setPosterFile] = useState<File | null>(null);
  const [posterPreview, setPosterPreview] = useState<string | null>(null);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadStatus, setUploadStatus] = useState<'idle' | 'uploading' | 'processing' | 'ready' | 'failed'>('idle');

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<MovieMetadataForm>({
    resolver: zodResolver(movieMetadataSchema),
  });

  const onDrop = useCallback((acceptedFiles: File[]) => {
    if (acceptedFiles.length > 0) {
      setUploadFile(acceptedFiles[0]);
      toast({
        title: 'File Selected',
        description: `${acceptedFiles[0].name} (${(acceptedFiles[0].size / 1024 / 1024).toFixed(2)} MB)`,
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

  const onSubmit = async (data: MovieMetadataForm) => {
    if (!uploadFile) {
      toast({
        title: 'Error',
        description: 'Please select a video file',
      });
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
        tags: data.tags ? data.tags.split(',').map((t) => t.trim()) : [],
        cast: data.cast ? data.cast.split(',').map((c) => c.trim()) : [],
        director: data.director,
      };

      const response = await uploadService.uploadMovie(
        uploadFile,
        metadata,
        (progress) => {
          setUploadProgress(progress);
        }
      );

      if (response.success) {
        setUploadStatus('processing');
        toast({
          title: 'Upload Complete',
          description: 'Your movie is being processed',
        });

        // Poll for processing status
        setTimeout(() => {
          setUploadStatus('ready');
          toast({
            title: 'Processing Complete',
            description: 'Your movie is now available',
          });
          reset();
          setUploadFile(null);
          setUploadProgress(0);
          setIsUploading(false);
        }, 3000);
      }
    } catch (error: any) {
      setUploadStatus('failed');
      toast({
        title: 'Upload Failed',
        description: error.message || 'An error occurred during upload',
      });
      setIsUploading(false);
    }
  };

  const removeFile = () => {
    setUploadFile(null);
    setUploadProgress(0);
  };

  return (
    <div className="container mx-auto px-4 py-8 max-w-4xl">
      <h1 className="text-4xl font-bold mb-8">Upload Movie</h1>

      <div className="space-y-8">
        {/* File Upload Area */}
        <div className="border-2 border-dashed rounded-lg p-8">
          {!uploadFile ? (
            <div {...getRootProps()} className="cursor-pointer">
              <input {...getInputProps()} />
              <div className="flex flex-col items-center justify-center space-y-4 py-12">
                <Upload className="h-16 w-16 text-muted-foreground" />
                <div className="text-center">
                  <p className="text-lg font-medium">
                    {isDragActive ? 'Drop the video file here' : 'Drag & drop a video file here'}
                  </p>
                  <p className="text-sm text-muted-foreground mt-2">
                    or click to browse (MP4, MKV, AVI, MOV, WMV)
                  </p>
                </div>
              </div>
            </div>
          ) : (
            <div className="flex items-center justify-between p-4 bg-secondary rounded-lg">
              <div className="flex items-center gap-4">
                <FileVideo className="h-8 w-8" />
                <div>
                  <p className="font-medium">{uploadFile.name}</p>
                  <p className="text-sm text-muted-foreground">
                    {(uploadFile.size / 1024 / 1024).toFixed(2)} MB
                  </p>
                </div>
              </div>
              {!isUploading && (
                <Button variant="ghost" size="icon" onClick={removeFile}>
                  <X className="h-5 w-5" />
                </Button>
              )}
            </div>
          )}

          {/* Upload Progress */}
          {isUploading && (
            <div className="mt-4 space-y-2">
              <Progress value={uploadProgress} />
              <p className="text-sm text-center text-muted-foreground">
                {uploadStatus === 'uploading' && `Uploading... ${uploadProgress}%`}
                {uploadStatus === 'processing' && 'Processing video...'}
                {uploadStatus === 'ready' && 'Complete!'}
                {uploadStatus === 'failed' && 'Upload failed'}
              </p>
            </div>
          )}
        </div>

        {/* Metadata Form */}
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid md:grid-cols-2 gap-6">
            <div className="space-y-2">
              <Label htmlFor="title">Title *</Label>
              <Input id="title" {...register('title')} />
              {errors.title && <p className="text-sm text-destructive">{errors.title.message}</p>}
            </div>

            <div className="space-y-2">
              <Label htmlFor="year">Year *</Label>
              <Input
                id="year"
                type="number"
                {...register('year', { valueAsNumber: true })}
              />
              {errors.year && <p className="text-sm text-destructive">{errors.year.message}</p>}
            </div>

            <div className="space-y-2">
              <Label htmlFor="duration">Duration (minutes) *</Label>
              <Input
                id="duration"
                type="number"
                {...register('duration', { valueAsNumber: true })}
              />
              {errors.duration && <p className="text-sm text-destructive">{errors.duration.message}</p>}
            </div>

            <div className="space-y-2">
              <Label htmlFor="genres">Genres (comma-separated) *</Label>
              <Input id="genres" placeholder="Action, Thriller, Drama" {...register('genres')} />
              {errors.genres && <p className="text-sm text-destructive">{errors.genres.message}</p>}
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="description">Description *</Label>
            <textarea
              id="description"
              className="flex min-h-[120px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              {...register('description')}
            />
            {errors.description && <p className="text-sm text-destructive">{errors.description.message}</p>}
          </div>

          <div className="space-y-2">
            <Label htmlFor="cast">Cast (comma-separated)</Label>
            <Input id="cast" placeholder="Actor 1, Actor 2, Actor 3" {...register('cast')} />
          </div>

          <div className="space-y-2">
            <Label htmlFor="director">Director</Label>
            <Input id="director" {...register('director')} />
          </div>

          <div className="space-y-2">
            <Label htmlFor="tags">Tags (comma-separated)</Label>
            <Input id="tags" placeholder="tag1, tag2, tag3" {...register('tags')} />
          </div>

          <div className="flex gap-4">
            <Button type="submit" disabled={!uploadFile || isUploading} className="flex-1">
              {isUploading ? 'Uploading...' : 'Upload Movie'}
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
      </div>
    </div>
  );
}
