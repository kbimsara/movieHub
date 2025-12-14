'use client';

import { useAuth } from '@/hooks/useAuth';
import { useAppSelector } from '@/hooks/redux';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Moon, Sun, Monitor } from 'lucide-react';

export default function SettingsPage() {
  const { user, logout } = useAuth();
  const { activeSeeds } = useAppSelector((state) => state.torrent);

  return (
    <div className="container mx-auto px-4 py-8 max-w-4xl">
      <h1 className="text-4xl font-bold mb-8">Settings</h1>

      <Tabs defaultValue="profile" className="space-y-6">
        <TabsList>
          <TabsTrigger value="profile">Profile</TabsTrigger>
          <TabsTrigger value="playback">Playback</TabsTrigger>
          <TabsTrigger value="torrent">Torrent</TabsTrigger>
          <TabsTrigger value="appearance">Appearance</TabsTrigger>
        </TabsList>

        {/* Profile Settings */}
        <TabsContent value="profile">
          <Card>
            <CardHeader>
              <CardTitle>Profile Information</CardTitle>
              <CardDescription>Manage your account details</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <p className="text-sm text-muted-foreground">Username</p>
                <p className="font-medium">{user?.username}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Email</p>
                <p className="font-medium">{user?.email}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Role</p>
                <p className="font-medium capitalize">{user?.role}</p>
              </div>
              <Button variant="destructive" onClick={logout}>
                Sign Out
              </Button>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Playback Settings */}
        <TabsContent value="playback">
          <Card>
            <CardHeader>
              <CardTitle>Playback Settings</CardTitle>
              <CardDescription>Customize your viewing experience</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="font-medium">Auto-play next episode</p>
                  <p className="text-sm text-muted-foreground">
                    Automatically play the next episode when one ends
                  </p>
                </div>
                <input type="checkbox" defaultChecked className="rounded" />
              </div>
              <div className="flex items-center justify-between">
                <div>
                  <p className="font-medium">Skip intro</p>
                  <p className="text-sm text-muted-foreground">
                    Automatically skip intros when available
                  </p>
                </div>
                <input type="checkbox" className="rounded" />
              </div>
              <div>
                <p className="font-medium mb-2">Default quality</p>
                <select className="w-full rounded-md border border-input bg-background px-3 py-2">
                  <option>Auto</option>
                  <option>1080p</option>
                  <option>720p</option>
                  <option>480p</option>
                </select>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Torrent Settings */}
        <TabsContent value="torrent">
          <Card>
            <CardHeader>
              <CardTitle>Torrent Settings</CardTitle>
              <CardDescription>Manage your seeding preferences</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div>
                <p className="text-sm text-muted-foreground mb-2">Active Seeds</p>
                <p className="text-3xl font-bold">{activeSeeds.length}</p>
              </div>
              <div className="flex items-center justify-between">
                <div>
                  <p className="font-medium">Auto-seed watched movies</p>
                  <p className="text-sm text-muted-foreground">
                    Automatically seed movies after watching
                  </p>
                </div>
                <input type="checkbox" className="rounded" />
              </div>
              <div>
                <p className="font-medium mb-2">Maximum upload speed</p>
                <select className="w-full rounded-md border border-input bg-background px-3 py-2">
                  <option>Unlimited</option>
                  <option>1 MB/s</option>
                  <option>500 KB/s</option>
                  <option>250 KB/s</option>
                </select>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Appearance Settings */}
        <TabsContent value="appearance">
          <Card>
            <CardHeader>
              <CardTitle>Appearance</CardTitle>
              <CardDescription>Customize the look and feel</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div>
                <p className="font-medium mb-4">Theme</p>
                <div className="grid grid-cols-3 gap-4">
                  <Button variant="outline" className="flex-col gap-2 h-auto py-4">
                    <Sun className="h-6 w-6" />
                    <span>Light</span>
                  </Button>
                  <Button variant="outline" className="flex-col gap-2 h-auto py-4">
                    <Moon className="h-6 w-6" />
                    <span>Dark</span>
                  </Button>
                  <Button variant="outline" className="flex-col gap-2 h-auto py-4">
                    <Monitor className="h-6 w-6" />
                    <span>System</span>
                  </Button>
                </div>
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
}
