'use client';

import { useState, useEffect } from 'react';
import { fileService, StorageStats } from '@/services/file.service';
import { FileManager } from '@/components/upload/FileManager';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import { useToast } from '@/components/ui/toaster';
import { 
  HardDrive, 
  FileVideo, 
  Image as ImageIcon, 
  FileText, 
  Upload,
  TrendingUp,
  Folder
} from 'lucide-react';
import { useRouter } from 'next/navigation';

export default function FilesPage() {
  const router = useRouter();
  const { toast } = useToast();
  const [stats, setStats] = useState<StorageStats | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    try {
      setIsLoading(true);
      const response = await fileService.getStorageStats();
      if (response.success && response.data) {
        setStats(response.data);
      }
    } catch (error: any) {
      // Handle 404 gracefully - backend not implemented yet
      if (error.response?.status === 404) {
        console.warn('File service endpoints not implemented on backend yet');
        // Set default stats for development
        setStats({
          totalFiles: 0,
          totalSize: 0,
          videoCount: 0,
          imageCount: 0,
          subtitleCount: 0,
          userQuota: 107374182400, // 100GB default
          usedQuota: 0,
        });
      } else {
        toast({
          title: 'Error',
          description: error.message || 'Failed to load storage stats',
        });
      }
    } finally {
      setIsLoading(false);
    }
  };

  const usagePercentage = stats 
    ? Math.round((stats.usedQuota / stats.userQuota) * 100)
    : 0;

  return (
    <div className="container mx-auto px-4 py-8 max-w-7xl">
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-4xl font-bold mb-2">File Manager</h1>
          <p className="text-muted-foreground">
            Manage your uploaded movies, posters, and media files
          </p>
        </div>
        <Button onClick={() => router.push('/upload')} size="lg">
          <Upload className="mr-2 h-5 w-5" />
          Upload Movie
        </Button>
      </div>

      {/* Backend Warning */}
      {stats && stats.totalFiles === 0 && (
        <div className="mb-6 p-4 bg-yellow-500/10 border border-yellow-500/20 rounded-lg flex items-start gap-3">
          <div className="p-2 rounded-full bg-yellow-500/10">
            <HardDrive className="h-5 w-5 text-yellow-500" />
          </div>
          <div className="text-sm flex-1">
            <p className="font-medium text-yellow-600 mb-1">Backend Setup Required</p>
            <p className="text-muted-foreground">
              File management endpoints need to be implemented. The frontend is ready, but the backend{' '}
              <code className="px-1 py-0.5 bg-muted rounded text-xs">/api/files/*</code> endpoints are not yet available.
              See <code className="px-1 py-0.5 bg-muted rounded text-xs">FILE_MANAGEMENT_SYSTEM.md</code> for details.
            </p>
          </div>
        </div>
      )}

      {/* Storage Overview */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {/* Total Storage */}
        <Card className="p-6">
          <div className="flex items-center gap-4">
            <div className="p-3 rounded-full bg-blue-500/10">
              <HardDrive className="h-6 w-6 text-blue-500" />
            </div>
            <div className="flex-1">
              <p className="text-sm text-muted-foreground">Total Storage</p>
              <p className="text-2xl font-bold">
                {stats ? fileService.formatFileSize(stats.totalSize) : '0 B'}
              </p>
            </div>
          </div>
        </Card>

        {/* Video Files */}
        <Card className="p-6">
          <div className="flex items-center gap-4">
            <div className="p-3 rounded-full bg-purple-500/10">
              <FileVideo className="h-6 w-6 text-purple-500" />
            </div>
            <div className="flex-1">
              <p className="text-sm text-muted-foreground">Video Files</p>
              <p className="text-2xl font-bold">
                {stats?.videoCount || 0}
              </p>
            </div>
          </div>
        </Card>

        {/* Image Files */}
        <Card className="p-6">
          <div className="flex items-center gap-4">
            <div className="p-3 rounded-full bg-green-500/10">
              <ImageIcon className="h-6 w-6 text-green-500" />
            </div>
            <div className="flex-1">
              <p className="text-sm text-muted-foreground">Image Files</p>
              <p className="text-2xl font-bold">
                {stats?.imageCount || 0}
              </p>
            </div>
          </div>
        </Card>

        {/* Subtitle Files */}
        <Card className="p-6">
          <div className="flex items-center gap-4">
            <div className="p-3 rounded-full bg-yellow-500/10">
              <FileText className="h-6 w-6 text-yellow-500" />
            </div>
            <div className="flex-1">
              <p className="text-sm text-muted-foreground">Subtitles</p>
              <p className="text-2xl font-bold">
                {stats?.subtitleCount || 0}
              </p>
            </div>
          </div>
        </Card>
      </div>

      {/* Storage Quota */}
      {stats && (
        <Card className="p-6 mb-8">
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <TrendingUp className="h-5 w-5 text-muted-foreground" />
                <div>
                  <h3 className="font-semibold">Storage Quota</h3>
                  <p className="text-sm text-muted-foreground">
                    {fileService.formatFileSize(stats.usedQuota)} of{' '}
                    {fileService.formatFileSize(stats.userQuota)} used
                  </p>
                </div>
              </div>
              <span className="text-2xl font-bold">{usagePercentage}%</span>
            </div>
            <Progress value={usagePercentage} className="h-2" />
            {usagePercentage > 80 && (
              <p className="text-sm text-yellow-600">
                ⚠️ You're running low on storage space. Consider deleting unused files.
              </p>
            )}
          </div>
        </Card>
      )}

      {/* File Manager */}
      <Card className="p-6">
        <div className="flex items-center gap-3 mb-6">
          <Folder className="h-5 w-5 text-muted-foreground" />
          <h2 className="text-2xl font-semibold">My Files</h2>
        </div>
        <FileManager />
      </Card>
    </div>
  );
}
