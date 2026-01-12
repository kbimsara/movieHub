'use client';

import { useState, useEffect } from 'react';
import { fileService, FileMetadata } from '@/services/file.service';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { useToast } from '@/components/ui/toaster';
import { 
  FileVideo, 
  Image as ImageIcon, 
  FileText, 
  Trash2, 
  Download, 
  Eye,
  Clock,
  HardDrive,
  RefreshCw 
} from 'lucide-react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';

interface FileManagerProps {
  movieId?: string;
  onFileSelect?: (file: FileMetadata) => void;
}

export function FileManager({ movieId, onFileSelect }: FileManagerProps) {
  const { toast } = useToast();
  const [files, setFiles] = useState<FileMetadata[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [selectedFile, setSelectedFile] = useState<FileMetadata | null>(null);
  const [showPreview, setShowPreview] = useState(false);

  useEffect(() => {
    loadFiles();
  }, [movieId]);

  const loadFiles = async () => {
    try {
      setIsLoading(true);
      const response = movieId 
        ? await fileService.getMovieFiles(movieId)
        : await fileService.getUserFiles();
      
      if (response.success && response.data) {
        setFiles(response.data);
      }
    } catch (error: any) {
      // Handle 404 gracefully - backend not implemented yet
      if (error.response?.status === 404) {
        console.warn('File service endpoints not implemented on backend yet');
        setFiles([]); // Show empty state instead of error
      } else {
        toast({
          title: 'Error',
          description: error.message || 'Failed to load files',
        });
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (fileId: string) => {
    if (!confirm('Are you sure you want to delete this file?')) return;

    try {
      await fileService.deleteFile(fileId);
      toast({
        title: 'Success',
        description: 'File deleted successfully',
      });
      loadFiles();
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error.message || 'Failed to delete file',
      });
    }
  };

  const handleDownload = async (file: FileMetadata) => {
    try {
      const response = await fileService.getDownloadUrl(file.id);
      if (response.success && response.data) {
        window.open(response.data.url, '_blank');
      }
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error.message || 'Failed to get download URL',
      });
    }
  };

  const getFileIcon = (type: string) => {
    switch (type) {
      case 'video':
        return <FileVideo className="h-8 w-8 text-blue-500" />;
      case 'image':
        return <ImageIcon className="h-8 w-8 text-green-500" />;
      case 'subtitle':
        return <FileText className="h-8 w-8 text-yellow-500" />;
      default:
        return <FileText className="h-8 w-8 text-gray-500" />;
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <RefreshCw className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Storage Stats */}
      <Card className="p-6">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <HardDrive className="h-5 w-5 text-muted-foreground" />
            <div>
              <h3 className="font-semibold">Storage</h3>
              <p className="text-sm text-muted-foreground">
                {files.length} files • {fileService.formatFileSize(files.reduce((sum, f) => sum + f.fileSize, 0))}
              </p>
            </div>
          </div>
          <Button variant="outline" size="sm" onClick={loadFiles}>
            <RefreshCw className="h-4 w-4 mr-2" />
            Refresh
          </Button>
        </div>
      </Card>

      {/* Files Grid */}
      {files.length === 0 ? (
        <Card className="p-12">
          <div className="text-center space-y-2">
            <FileVideo className="h-12 w-12 mx-auto text-muted-foreground" />
            <h3 className="font-semibold">No files yet</h3>
            <p className="text-sm text-muted-foreground">
              Upload files to see them here
            </p>
          </div>
        </Card>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {files.map((file) => (
            <Card key={file.id} className="p-4 hover:shadow-lg transition-shadow">
              <div className="space-y-3">
                {/* File Icon & Type */}
                <div className="flex items-start justify-between">
                  <div className="flex items-center gap-3">
                    {getFileIcon(file.fileType)}
                    <div className="flex-1 min-w-0">
                      <p className="font-medium truncate" title={file.originalName}>
                        {file.originalName}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        {fileService.formatFileSize(file.fileSize)}
                      </p>
                    </div>
                  </div>
                </div>

                {/* File Details */}
                <div className="space-y-1 text-xs text-muted-foreground">
                  {file.duration && (
                    <div className="flex items-center gap-1">
                      <Clock className="h-3 w-3" />
                      <span>{Math.floor(file.duration / 60)}m {file.duration % 60}s</span>
                    </div>
                  )}
                  {file.width && file.height && (
                    <p>{file.width} × {file.height}</p>
                  )}
                  <p>{formatDate(file.uploadedAt)}</p>
                </div>

                {/* Thumbnail Preview */}
                {file.thumbnailUrl && (
                  <div className="aspect-video rounded overflow-hidden bg-muted">
                    <img 
                      src={file.thumbnailUrl} 
                      alt={file.originalName}
                      className="w-full h-full object-cover"
                    />
                  </div>
                )}

                {/* Actions */}
                <div className="flex gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    className="flex-1"
                    onClick={() => {
                      setSelectedFile(file);
                      setShowPreview(true);
                    }}
                  >
                    <Eye className="h-4 w-4 mr-1" />
                    View
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handleDownload(file)}
                  >
                    <Download className="h-4 w-4" />
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handleDelete(file.id)}
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}

      {/* File Preview Dialog */}
      <Dialog open={showPreview} onOpenChange={setShowPreview}>
        <DialogContent className="max-w-4xl">
          <DialogHeader>
            <DialogTitle>{selectedFile?.originalName}</DialogTitle>
          </DialogHeader>
          {selectedFile && (
            <div className="space-y-4">
              {selectedFile.fileType === 'video' && (
                <video 
                  src={selectedFile.url} 
                  controls 
                  className="w-full rounded"
                />
              )}
              {selectedFile.fileType === 'image' && (
                <img 
                  src={selectedFile.url} 
                  alt={selectedFile.originalName}
                  className="w-full rounded"
                />
              )}
              
              {/* File Details */}
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <p className="text-muted-foreground">File Size</p>
                  <p className="font-medium">{fileService.formatFileSize(selectedFile.fileSize)}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Type</p>
                  <p className="font-medium">{selectedFile.mimeType}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Uploaded</p>
                  <p className="font-medium">{formatDate(selectedFile.uploadedAt)}</p>
                </div>
                {selectedFile.duration && (
                  <div>
                    <p className="text-muted-foreground">Duration</p>
                    <p className="font-medium">
                      {Math.floor(selectedFile.duration / 60)}m {selectedFile.duration % 60}s
                    </p>
                  </div>
                )}
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}
