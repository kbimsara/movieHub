'use client';

import { UploadProgress } from '@/types';
import { Card } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import { Button } from '@/components/ui/button';
import { FileVideo, CheckCircle2, XCircle, Loader2, X } from 'lucide-react';

interface UploadProgressCardProps {
  upload: UploadProgress;
  onCancel?: (id: string) => void;
  onRetry?: (id: string) => void;
}

export function UploadProgressCard({ upload, onCancel, onRetry }: UploadProgressCardProps) {
  const getStatusIcon = () => {
    switch (upload.status) {
      case 'uploading':
        return <Loader2 className="h-5 w-5 animate-spin text-blue-500" />;
      case 'processing':
        return <Loader2 className="h-5 w-5 animate-spin text-yellow-500" />;
      case 'ready':
        return <CheckCircle2 className="h-5 w-5 text-green-500" />;
      case 'failed':
        return <XCircle className="h-5 w-5 text-red-500" />;
      default:
        return <FileVideo className="h-5 w-5" />;
    }
  };

  const getStatusText = () => {
    switch (upload.status) {
      case 'uploading':
        return 'Uploading...';
      case 'processing':
        return 'Processing...';
      case 'ready':
        return 'Complete';
      case 'failed':
        return 'Failed';
      default:
        return 'Pending';
    }
  };

  const getStatusColor = () => {
    switch (upload.status) {
      case 'uploading':
        return 'text-blue-500';
      case 'processing':
        return 'text-yellow-500';
      case 'ready':
        return 'text-green-500';
      case 'failed':
        return 'text-red-500';
      default:
        return 'text-muted-foreground';
    }
  };

  return (
    <Card className="p-4">
      <div className="flex items-start gap-4">
        <div className="mt-1">{getStatusIcon()}</div>
        
        <div className="flex-1 min-w-0">
          <div className="flex items-start justify-between gap-2 mb-2">
            <div className="flex-1 min-w-0">
              <h3 className="font-medium truncate">{upload.fileName}</h3>
              <p className="text-sm text-muted-foreground">
                {(upload.fileSize / 1024 / 1024).toFixed(2)} MB
              </p>
            </div>
            <div className="flex items-center gap-2">
              <span className={`text-sm font-medium ${getStatusColor()}`}>
                {getStatusText()}
              </span>
              {upload.status === 'uploading' && onCancel && (
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8"
                  onClick={() => onCancel(upload.id)}
                >
                  <X className="h-4 w-4" />
                </Button>
              )}
            </div>
          </div>

          {(upload.status === 'uploading' || upload.status === 'processing') && (
            <div className="space-y-1">
              <Progress value={upload.progress} className="h-2" />
              <p className="text-xs text-muted-foreground text-right">
                {upload.progress}%
              </p>
            </div>
          )}

          {upload.status === 'failed' && upload.error && (
            <div className="space-y-2">
              <p className="text-sm text-red-500">{upload.error}</p>
              {onRetry && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => onRetry(upload.id)}
                >
                  Retry Upload
                </Button>
              )}
            </div>
          )}

          {upload.status === 'ready' && upload.movieId && (
            <div className="flex gap-2 mt-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => window.open(`/movie/${upload.movieId}`, '_blank')}
              >
                View Movie
              </Button>
            </div>
          )}
        </div>
      </div>
    </Card>
  );
}
