'use client';

import { Provider } from 'react-redux';
import { store } from '@/store';
import { Toaster } from '@/components/ui/toaster';
import Navbar from '@/components/layout/Navbar';
import { SessionMonitor } from '@/components/layout/SessionMonitor';
import './globals.css';

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" className="dark">
      <body>
        <Provider store={store}>
          <SessionMonitor />
          <Toaster>
            <Navbar />
            <main className="pt-16 min-h-screen">
              {children}
            </main>
          </Toaster>
        </Provider>
      </body>
    </html>
  );
}
