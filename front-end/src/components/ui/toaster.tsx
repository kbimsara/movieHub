'use client';

import * as React from 'react';
import { Toast, ToastProvider, ToastViewport } from '@/components/ui/toast';

type ToastType = 'default' | 'success' | 'error' | 'warning';

interface ToastMessage {
  id: string;
  title?: string;
  description?: string;
  type?: ToastType;
}

interface ToasterContextType {
  toast: (message: Omit<ToastMessage, 'id'>) => void;
}

const ToasterContext = React.createContext<ToasterContextType | undefined>(undefined);

export function Toaster({ children }: { children: React.ReactNode }) {
  const [toasts, setToasts] = React.useState<ToastMessage[]>([]);

  const toast = React.useCallback((message: Omit<ToastMessage, 'id'>) => {
    const id = Math.random().toString(36).substring(7);
    setToasts((prev) => [...prev, { ...message, id }]);
    setTimeout(() => {
      setToasts((prev) => prev.filter((t) => t.id !== id));
    }, 3000);
  }, []);

  return (
    <ToasterContext.Provider value={{ toast }}>
      <ToastProvider>
        {children}
        <ToastViewport />
        {toasts.map((toast) => (
          <Toast key={toast.id}>
            <div className="grid gap-1">
              {toast.title && <div className="font-semibold">{toast.title}</div>}
              {toast.description && <div className="text-sm opacity-90">{toast.description}</div>}
            </div>
          </Toast>
        ))}
      </ToastProvider>
    </ToasterContext.Provider>
  );
}

export function useToast() {
  const context = React.useContext(ToasterContext);
  if (!context) {
    throw new Error('useToast must be used within a Toaster');
  }
  return context;
}
